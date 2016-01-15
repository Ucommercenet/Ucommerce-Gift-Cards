properties {
  $deployment_directory = $TargetDirectory;
  $WorkDictionary = $SourceDirectory;
}

function FileExtensionBlackList {
  return "*.cd","*.cs","*.dll","*.xml","*obj*","*.pdb","*.csproj*","*.cache","*.orig";  
}

function DllExtensionBlackList {
  return "UCommerce.*","Castle.Core.dll","Castle.Windsor.dll","ClientDependency.Core.dll","Esent.Interop.dll","FluentNHibernate.dll","ICSharpCode.NRefactory.dll","ICSharpCode.NRefactory.CSharp.dll","Iesi.Collections.dll","Jint.Raven.dll","log4net.dll","Lucene.Net.dll","Lucene.Net.Contrib.Spatial.NTS.dll","Microsoft.CompilerServices.AsyncTargetingPack.Net4.dll","Microsoft.WindowsAzure.Storage.dll","NHibernate.dll","Raven.Abstractions.dll","Raven.Client.Embedded.dll","Raven.Client.Lightweight.dll","Raven.Database.dll","ServiceStack.Common.dll";"ServiceStack.dll","ServiceStack.Interfaces.dll","ServiceStack.ServiceInterface.dll","ServiceStack.Text.dll","Spatial4n.Core.NTS.dll";  
}

function GetFilesToCopy($path){
	return Get-ChildItem $path -name -recurse -include *.* -exclude (FileExtensionBlackList);
}

function CopyFiles ($appDirectory) {
	write-host 'copying files from: ' $WorkDictionary	
	write-host 'copying files to: ' $appDirectory;
	
	$filesToCopy = GetFilesToCopy($WorkDictionary);
	 

    if(!$filesToCopy)
    {
      return;
    }
	
	foreach($fileToCopy in $filesToCopy)
	{
        $sourceFile = $WorkDictionary + "\" + $fileToCopy;
		$targetFile = $appDirectory + "\" + $fileToCopy;
		
		# Create the folder structure and empty destination file,
		New-Item -ItemType File -Path $targetFile -Force
		Write-Host 'copying' $targetFile
		Copy-Item $sourceFile $targetFile -Force
	}
}


function GetDllesToCopy($path){
	return Get-ChildItem $path -name -recurse -include "*.dll*","*.pdb*"  -exclude (DllExtensionBlackList);
}

function CopyDllToBin ($appDirectory) {

    
	$filesToCopy = GetDllesToCopy($WorkDictionary);

	foreach($fileToCopy in $filesToCopy)
	{
        if($fileToCopy -notlike '*obj*'){
		    $sourceFile = $WorkDictionary + "\" + $fileToCopy;
		    $targetFile = $appDirectory + "\" + $fileToCopy;	
		
		    # Create the folder structure and empty destination file,
		    New-Item -ItemType File -Path $targetFile -Force	
		
		    Copy-Item $sourceFile $targetFile -Force
            #& robocopy $sourceFile $targetFile /S /XF *.cs /xf *.user /xf *.old /xf *.vspscc /xf xsltExtensions.config /xf uCommerce.key /xf *.tmp /xd _Resharper* /xd .svn /xd _svn /xf *.orig /E /NFL /NDL
    
        }
	}
}

task Run-It {
        
	write-host 'Extracting app to' + $deployment_directory;
    
    #Creates app directory
    If (!(Test-Path $deployment_directory)) {
		write-host 'Creating directory: ' + $deployment_directory;
	    New-Item $deployment_directory -type directory 
    }	
	
	CopyFiles($deployment_directory);

	CopyDllToBin($deployment_directory);
    
    write-host 'Extracted app to' + $deployment_directory;   
}

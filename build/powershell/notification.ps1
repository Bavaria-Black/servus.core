Write-Output "Prepare message..."
$request = @{
    content = "
**$Env:BUILD_REPOSITORY_NAME $Env:BUILD_BUILDNUMBER** is now available!
``````
$Env:BUILD_SOURCEVERSIONMESSAGE
``````"
}

$content = ConvertTo-Json $request
$path = "$Env:BUILD_ARTIFACTSTAGINGDIRECTORY/notification.txt"
Write-Output "Write to file..."
Write-Output $path

Set-Content -Path $path -Value $content

Write-Output $content
Write-Output "Done!"

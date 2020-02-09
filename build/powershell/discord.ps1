$request = @{
    content = "
**$Env:BUILD_REPOSITORY_NAME $Env:BUILD_BUILDNUMBER** is now available!
``````
$Env:BUILD_SOURCEVERSIONMESSAGE
``````"
}

$content = ConvertTo-Json $request

Write-Output $content
Invoke-WebRequest -Method POST -Uri $env:DISCORD -ContentType "application/json" -Body $content
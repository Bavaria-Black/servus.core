$request = @{
    content = "
**$(Build.BuildNumber)** is now available!
``````
$(Build.SourceVersionMessage)
``````"
}

$content = ConvertTo-Json $request

Write-Output $content
Invoke-WebRequest -Method POST -Uri $env:DISCORD -ContentType "application/json" -Body $content
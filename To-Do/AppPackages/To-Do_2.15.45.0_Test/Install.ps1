﻿#
# This script just calls the Add-AppDevPackage.ps1 script that lives next to it.
#

param(
    [switch]$Force = $false,
    [switch]$SkipLoggingTelemetry = $false
)

$scriptArgs = ""
if ($Force)
{
    $scriptArgs = '-Force'
}

if ($SkipLoggingTelemetry)
{
    if ($Force)
    {
        $scriptArgs += ' '
    }

    $scriptArgs += '-SkipLoggingTelemetry'
}

try
{
    # Log telemetry data regarding the use of the script if possible.
    # There are three ways that this can be disabled:
    #   1. If the "TelemetryDependencies" folder isn't present.  This can be excluded at build time by setting the MSBuild property AppxLogTelemetryFromSideloadingScript to false
    #   2. If the SkipLoggingTelemetry switch is passed to this script.
    #   3. If Visual Studio telemetry is disabled via the registry.
    $TelemetryAssembliesFolder = (Join-Path $PSScriptRoot "TelemetryDependencies")
    if (!$SkipLoggingTelemetry -And (Test-Path $TelemetryAssembliesFolder))
    {
        $job = Start-Job -FilePath (Join-Path $TelemetryAssembliesFolder "LogSideloadingTelemetry.ps1") -ArgumentList $TelemetryAssembliesFolder, "VS/DesignTools/SideLoadingScript/Install", $null, $null
        Wait-Job -Job $job -Timeout 60 | Out-Null
    }
}
catch
{
    # Ignore telemetry errors
}

$currLocation = Get-Location
Set-Location $PSScriptRoot
Invoke-Expression ".\Add-AppDevPackage.ps1 $scriptArgs"
Set-Location $currLocation
# SIG # Begin signature block
# MIIhggYJKoZIhvcNAQcCoIIhczCCIW8CAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCC7kxV/l3biwCGH
# VuAKUAkPVeCZ2LSQIMJf+ROzV3B37KCCC3IwggT6MIID4qADAgECAhMzAAAD3o1W
# glrxpKlnAAAAAAPeMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTAwHhcNMjAxMjE1MjEyNDIwWhcNMjExMjAyMjEyNDIwWjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQCn8/DiSdIkSLNe/aikoxkWIAtCq9W+7+dbEXN/cZ4ZkeX/mn8MPRJVyt7FaE3k
# bFofDl4GH5xZOQoOUhGh8mfq3sUq9RDZ6T1vFanVTeRq3iamvmjYHVgYq1MW9RdY
# 1UaGH7OE2L0coGjp46azVnYJlj7I1BWMZXxYRSxGQttNi9g1lwXCGfGl+aTqWSfy
# hRpMB/mpVC4IF8MYsHVYctYb1ex+3ZwDSmwsX/Y4sxtSpt18xX9Rso2fES8GGnQK
# dBnMMdySgdp0E4ACWpa0iQDTBQMQVSTfrVFjyetviHoMejI+SgbkiPrEezGGvTiG
# Ooc6+eWSEwYMy9sxw0GaCiVvAgMBAAGjggF5MIIBdTAfBgNVHSUEGDAWBgorBgEE
# AYI3PQYBBggrBgEFBQcDAzAdBgNVHQ4EFgQUJOo70fFaruzlxG7u+R2Hd9N1TIkw
# UAYDVR0RBEkwR6RFMEMxKTAnBgNVBAsTIE1pY3Jvc29mdCBPcGVyYXRpb25zIFB1
# ZXJ0byBSaWNvMRYwFAYDVQQFEw0yMzA4NjUrNDYzMTMzMB8GA1UdIwQYMBaAFOb8
# X3u7IgBY5HJOtfQhdCMy5u+sMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwu
# bWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNpZ1BDQV8yMDEw
# LTA3LTA2LmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93
# d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENBXzIwMTAtMDct
# MDYuY3J0MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggEBADxf8A60TSfr
# Xa5t5G1ExJ08OpjCi9RBsN8T+wkozcz1E0QfB8mxaE38u00S89z+HoCJ5/LCgMBY
# VoTWAHtbG4PYT7rLcGzBuX8S+OIlBR2LtghZ5H6mCiblpD/3dMz2d2Z62/TCYAHp
# 0iKlHa+EJ/KW7SPsvfUNxQ2IbolvXfQGuTyFV9HEN/NdfmWWg78vXVZ25TlM1HLj
# uotGgyG7Td3b4cHQBZbO+XVZ/AITsfAB6TlqFzGCLINYyZ90EX0sAcm6mVvO2GDU
# PiqJjKoh+0nO06ccETCeyMMRxt6oyJB+0UkldpKX4aZc18u0gNohHGx04bp9Jc6D
# TuTh3ynE8hwwggZwMIIEWKADAgECAgphDFJMAAAAAAADMA0GCSqGSIb3DQEBCwUA
# MIGIMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMTIwMAYDVQQD
# EylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkgMjAxMDAeFw0x
# MDA3MDYyMDQwMTdaFw0yNTA3MDYyMDUwMTdaMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTAwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDpDmRQ
# eWe1xOP9CQBMnpSs91Zo6kTYz8VYT6mldnxtRbrTOZK0pB75+WWC5BfSj/1EnAjo
# ZZPOLFWEv30I4y4rqEErGLeiS25JTGsVB97R0sKJHnGUzbV/S7SvCNjMiNZrF5Q6
# k84mP+zm/jSYV9UdXUn2siou1YW7WT/4kLQrg3TKK7M7RuPwRknBF2ZUyRy9HcRV
# Yldy+Ge5JSA03l2mpZVeqyiAzdWynuUDtWPTshTIwciKJgpZfwfs/w7tgBI1TBKm
# vlJb9aba4IsLSHfWhUfVELnG6Krui2otBVxgxrQqW5wjHF9F4xoUHm83yxkzgGqJ
# TaNqZmN4k9Uwz5UfAgMBAAGjggHjMIIB3zAQBgkrBgEEAYI3FQEEAwIBADAdBgNV
# HQ4EFgQU5vxfe7siAFjkck619CF0IzLm76wwGQYJKwYBBAGCNxQCBAweCgBTAHUA
# YgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAU
# 1fZWy4/oolxiaNE9lJBb186aGMQwVgYDVR0fBE8wTTBLoEmgR4ZFaHR0cDovL2Ny
# bC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljUm9vQ2VyQXV0XzIw
# MTAtMDYtMjMuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggrBgEFBQcwAoY+aHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXRfMjAxMC0w
# Ni0yMy5jcnQwgZ0GA1UdIASBlTCBkjCBjwYJKwYBBAGCNy4DMIGBMD0GCCsGAQUF
# BwIBFjFodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vUEtJL2RvY3MvQ1BTL2RlZmF1
# bHQuaHRtMEAGCCsGAQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAFAAbwBsAGkAYwB5
# AF8AUwB0AGEAdABlAG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQAadO9X
# Tyl7xBaFeLhQ0yL8CZ2sgpf4NP8qLJeVEuXkv8+/k8jjNKnbgbjcHgC+0jVvr+V/
# eZV35QLU8evYzU4eG2GiwlojGvCMqGJRRWcI4z88HpP4MIUXyDlAptcOsyEp5aWh
# aYwik8x0mOehR0PyU6zADzBpf/7SJSBtb2HT3wfV2XIALGmGdj1R26Y5SMk3YW0H
# 3VMZy6fWYcK/4oOrD+Brm5XWfShRsIlKUaSabMi3H0oaDmmp19zBftFJcKq2rbty
# R2MX+qbWoqaG7KgQRJtjtrJpiQbHRoZ6GD/oxR0h1Xv5AiMtxUHLvx1MyBbvsZx/
# /CJLSYpuFeOmf3Zb0VN5kYWd1dLbPXM18zyuVLJSR2rAqhOV0o4R2plnXjKM+zeF
# 0dx1hZyHxlpXhcK/3Q2PjJst67TuzyfTtV5p+qQWBAGnJGdzz01Ptt4FVpd69+lS
# TfR3BU+FxtgL8Y7tQgnRDXbjI1Z4IiY2vsqxjG6qHeSF2kczYo+kyZEzX3EeQK+Y
# Zcki6EIhJYocLWDZN4lBiSoWD9dhPJRoYFLv1keZoIBA7hWBdz6c4FMYGlAdOJWb
# HmYzEyc5F3iHNs5Ow1+y9T1HU7bg5dsLYT0q15IszjdaPkBCMaQfEAjCVpy/JF1R
# Ap1qedIX09rBlI4HeyVxRKsGaubUxt8jmpZ1xTGCFWYwghViAgEBMIGVMH4xCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jv
# c29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMTACEzMAAAPejVaCWvGkqWcAAAAAA94w
# DQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYK
# KwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEILnmx/DL
# sme4EXSNG1kZGesXMcnWcLyJHy2/07NYyjrlMEIGCisGAQQBgjcCAQwxNDAyoBSA
# EgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20w
# DQYJKoZIhvcNAQEBBQAEggEAliy5Xflg0gc2+bFVkRrRPHCOyZzptblstBMClPgb
# yDocUlaxsxhhrRxAZ3VxCWgmaO5ST1DxIXBNoFYqRKOf0hbCLPxbCaJ14MUVmN2w
# Ts5okJTM2o88uwa2iZ7du3lj8HQbOUSyM8bQINAKl99P2Kp2audZXP5VlQJlW4cI
# rOgs7TX/ze0Ib0SDiA1qoJlrpC2iyYiF+eimc5vsCuD2uTeiO9m985KBRJ3AcqUU
# akv8PS/K4/9pTz+XMR9VUk2g7KNESOizF6vCF8rGxbuZ9VbvMBFwYc5Pu+rp6Tq1
# LscMLl5juNnpc/Otq8pNr+gGFJS1V0OG/2oruTMNEgMZKaGCEvAwghLsBgorBgEE
# AYI3AwMBMYIS3DCCEtgGCSqGSIb3DQEHAqCCEskwghLFAgEDMQ8wDQYJYIZIAWUD
# BAIBBQAwggFUBgsqhkiG9w0BCRABBKCCAUMEggE/MIIBOwIBAQYKKwYBBAGEWQoD
# ATAxMA0GCWCGSAFlAwQCAQUABCA/mD9E/Sz0ioy3aBqrBsXZ4NGDHVIKTcxshyv2
# zLLJPgIGYR6724FDGBIyMDIxMDkxMDAzNDIzNC44NlowBIACAfSggdSkgdEwgc4x
# CzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRt
# b25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKTAnBgNVBAsTIE1p
# Y3Jvc29mdCBPcGVyYXRpb25zIFB1ZXJ0byBSaWNvMSYwJAYDVQQLEx1UaGFsZXMg
# VFNTIEVTTjpGN0E2LUUyNTEtMTUwQTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUt
# U3RhbXAgU2VydmljZaCCDkQwggT1MIID3aADAgECAhMzAAABWZ/8fl8s6vJDAAAA
# AAFZMA0GCSqGSIb3DQEBCwUAMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEw
# MB4XDTIxMDExNDE5MDIxNVoXDTIyMDQxMTE5MDIxNVowgc4xCzAJBgNVBAYTAlVT
# MRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQK
# ExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKTAnBgNVBAsTIE1pY3Jvc29mdCBPcGVy
# YXRpb25zIFB1ZXJ0byBSaWNvMSYwJAYDVQQLEx1UaGFsZXMgVFNTIEVTTjpGN0E2
# LUUyNTEtMTUwQTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2Vydmlj
# ZTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAK54xGHJZ8SHREtNIoBo
# 9AG6Mro8gEZCt8WgV/mNdIt2tMOP3zVYU4+sRsImxTwfzJEDBWaTc7LxlEy/1302
# fRmd/R2pwnY7pyT90yvZAmQQLZ6D+faGBwwhi5rre/tmBJdbAXFZ8qL2JDc4txBn
# 30Mr1C8DFBdrIjwbP+i2RdAOaSwIs/xQsMeZAz3v5j9VEdwq8+iM6YcLcqKrYAwP
# +OE58371ST5kj2f7quToeTXhSvDczKYrVokL3Zn0+KNAnbpp4rH1tXymmgXQcgVC
# z1E/Ey8NEsvZ1FjV5QP6ovDMT8YAo7KzaYvT4Ix+xMVvW+1/1MnYaaoR8bLnQxmT
# ZOMCAwEAAaOCARswggEXMB0GA1UdDgQWBBT20KmFRryt+uTrJ9eIwjyy6Tdj5zAf
# BgNVHSMEGDAWgBTVYzpcijGQ80N7fEYbxTNoWoVtVTBWBgNVHR8ETzBNMEugSaBH
# hkVodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNU
# aW1TdGFQQ0FfMjAxMC0wNy0wMS5jcmwwWgYIKwYBBQUHAQEETjBMMEoGCCsGAQUF
# BzAChj5odHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY1RpbVN0
# YVBDQV8yMDEwLTA3LTAxLmNydDAMBgNVHRMBAf8EAjAAMBMGA1UdJQQMMAoGCCsG
# AQUFBwMIMA0GCSqGSIb3DQEBCwUAA4IBAQCNkVQS6A+BhrfGOCAWo3KcuUa4estp
# zyn+ZLlkh0pJmAJp4EUDrLWsieYCf2oyoc8KjVMC+NHFFVvHLrSMhWnR5FtY6l3Z
# 6Ur9ITBSz64j5wTRRE8vIpQiHVYjRVNPGR2tiqG5nKP5+sD0rZI464OFNz4n7erD
# JOpV7Im1L/sAwfX+GHoc4j5rfuAuQTFY82sdYvtHM4LTxwV997uhlFs52oHapdFW
# 1KXt6vMxEXnSX8soQfUd+M+Yq3J7udc6R941Guxfd6A0vecV56JjvmpCng4jRkqu
# Aeyf/dKmQUaR1fKvALBRAmZkAUtWijS/3MkeQv/lUvHVo7GPFzJ/O3wJMIIGcTCC
# BFmgAwIBAgIKYQmBKgAAAAAAAjANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UEBhMC
# VVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNV
# BAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJv
# b3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIwMTAwHhcNMTAwNzAxMjEzNjU1WhcN
# MjUwNzAxMjE0NjU1WjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3Rv
# bjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0
# aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDCCASIw
# DQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKkdDbx3EYo6IOz8E5f1+n9plGt0
# VBDVpQoAgoX77XxoSyxfxcPlYcJ2tz5mK1vwFVMnBDEfQRsalR3OCROOfGEwWbEw
# RA/xYIiEVEMM1024OAizQt2TrNZzMFcmgqNFDdDq9UeBzb8kYDJYYEbyWEeGMoQe
# dGFnkV+BVLHPk0ySwcSmXdFhE24oxhr5hoC732H8RsEnHSRnEnIaIYqvS2SJUGKx
# Xf13Hz3wV3WsvYpCTUBR0Q+cBj5nf/VmwAOWRH7v0Ev9buWayrGo8noqCjHw2k4G
# kbaICDXoeByw6ZnNPOcvRLqn9NxkvaQBwSAJk3jN/LzAyURdXhacAQVPIk0CAwEA
# AaOCAeYwggHiMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBTVYzpcijGQ80N7
# fEYbxTNoWoVtVTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMC
# AYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBTV9lbLj+iiXGJo0T2UkFvX
# zpoYxDBWBgNVHR8ETzBNMEugSaBHhkVodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20v
# cGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJBdXRfMjAxMC0wNi0yMy5jcmwwWgYI
# KwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5odHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20vcGtpL2NlcnRzL01pY1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNydDCBoAYDVR0g
# AQH/BIGVMIGSMIGPBgkrBgEEAYI3LgMwgYEwPQYIKwYBBQUHAgEWMWh0dHA6Ly93
# d3cubWljcm9zb2Z0LmNvbS9QS0kvZG9jcy9DUFMvZGVmYXVsdC5odG0wQAYIKwYB
# BQUHAgIwNB4yIB0ATABlAGcAYQBsAF8AUABvAGwAaQBjAHkAXwBTAHQAYQB0AGUA
# bQBlAG4AdAAuIB0wDQYJKoZIhvcNAQELBQADggIBAAfmiFEN4sbgmD+BcQM9naOh
# IW+z66bM9TG+zwXiqf76V20ZMLPCxWbJat/15/B4vceoniXj+bzta1RXCCtRgkQS
# +7lTjMz0YBKKdsxAQEGb3FwX/1z5Xhc1mCRWS3TvQhDIr79/xn/yN31aPxzymXlK
# kVIArzgPF/UveYFl2am1a+THzvbKegBvSzBEJCI8z+0DpZaPWSm8tv0E4XCfMkon
# /VWvL/625Y4zu2JfmttXQOnxzplmkIz/amJ/3cVKC5Em4jnsGUpxY517IW3DnKOi
# PPp/fZZqkHimbdLhnPkd/DjYlPTGpQqWhqS9nhquBEKDuLWAmyI4ILUl5WTs9/S/
# fmNZJQ96LjlXdqJxqgaKD4kWumGnEcua2A5HmoDF0M2n0O99g/DhO3EJ3110mCII
# YdqwUB5vvfHhAN/nMQekkzr3ZUd46PioSKv33nJ+YWtvd6mBy6cJrDm77MbL2IK0
# cs0d9LiFAR6A+xuJKlQ5slvayA1VmXqHczsI5pgt6o3gMy4SKfXAL1QnIffIrE7a
# KLixqduWsqdCosnPGUFN4Ib5KpqjEWYw07t0MkvfY3v1mYovG8chr1m1rtxEPJdQ
# cdeh0sVV42neV8HR3jDA/czmTfsNv11P6Z0eGTgvvM9YBS7vDaBQNdrvCScc1bN+
# NR4Iuto229Nfj950iEkSoYIC0jCCAjsCAQEwgfyhgdSkgdEwgc4xCzAJBgNVBAYT
# AlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYD
# VQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKTAnBgNVBAsTIE1pY3Jvc29mdCBP
# cGVyYXRpb25zIFB1ZXJ0byBSaWNvMSYwJAYDVQQLEx1UaGFsZXMgVFNTIEVTTjpG
# N0E2LUUyNTEtMTUwQTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2Vy
# dmljZaIjCgEBMAcGBSsOAwIaAxUAKnbLAI8fhO58SCWrpZnXvXEZshGggYMwgYCk
# fjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQD
# Ex1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDANBgkqhkiG9w0BAQUFAAIF
# AOTk6VYwIhgPMjAyMTA5MTAwMDEzMTBaGA8yMDIxMDkxMTAwMTMxMFowdzA9Bgor
# BgEEAYRZCgQBMS8wLTAKAgUA5OTpVgIBADAKAgEAAgImVAIB/zAHAgEAAgIRKjAK
# AgUA5OY61gIBADA2BgorBgEEAYRZCgQCMSgwJjAMBgorBgEEAYRZCgMCoAowCAIB
# AAIDB6EgoQowCAIBAAIDAYagMA0GCSqGSIb3DQEBBQUAA4GBAAArCP0yqAkqvp6x
# kndDqEWuKuOGsqvTJ5ntFJ0BjpYuVPcXAOG21tCzPsbjuy/hbD/uxroldxX9/ac+
# wuvyCQTOoQYgYlupDK0IbGGmQ6OoT0uXl39qP1csVyywJnoAkLGAhPDjDsxt5yHH
# yaLh5D4ui0aUjy4aPL1Uslwy2oifMYIDDTCCAwkCAQEwgZMwfDELMAkGA1UEBhMC
# VVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNV
# BAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRp
# bWUtU3RhbXAgUENBIDIwMTACEzMAAAFZn/x+Xyzq8kMAAAAAAVkwDQYJYIZIAWUD
# BAIBBQCgggFKMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0BCRABBDAvBgkqhkiG9w0B
# CQQxIgQg9/xi6i8d0+huWTABa0KHmLxJLtdOZ5IA76/8j5nea/UwgfoGCyqGSIb3
# DQEJEAIvMYHqMIHnMIHkMIG9BCABWBvPvzDmfNeSzmJT4+dGA+uj/qq7/fKkUn36
# rxND6DCBmDCBgKR+MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAAB
# WZ/8fl8s6vJDAAAAAAFZMCIEIPFi+gM1tpHYTgi2tjQc6CJP/Xbq9hjRYDHobo59
# 43cgMA0GCSqGSIb3DQEBCwUABIIBAHhJo2bLQR0kG9pIWIlP+Sj2D2Uc2ckfWxxD
# bzNRCMIipVVhbwSzynfwx855ML4ezMsU3qKmMFKO7ABYV4+kwMqjR+wrXNnpN1Ge
# /0XJ4+In/GfxKkJX5T9BiHosDnmwFky/xXdBSoID1/ka7QMUwe+vNfLQhMSBNF3a
# yI0KN2OdYcvsezlgbQINWrlikGrJpp1MRyJTz75OEnN71za8VrrGahlL/OMsx56W
# haKVdD7ZW3nv3WrS5xmLUeruGQ/3YKHMGP97ok8DIHLnjP8g+9UVBswH7b7NyPqk
# wTENodwCmO9OoRlHZeT+4l/9yhoWl3oiEIbjXqdhMpOdOMbevBg=
# SIG # End signature block

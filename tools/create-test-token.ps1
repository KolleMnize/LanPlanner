param([string]$Email = "test@example.com")

# Muss mit appsettings.Development.json übereinstimmen
$secret   = "ein-langer-lokaler-testschluessel-mind-32-zeichen"
$issuer   = "local-dev"
$audience = "lanplanner-local"

function ConvertTo-Base64Url([byte[]]$bytes) {
    [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')
}

$header = ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes('{"alg":"HS256","typ":"JWT"}'))

$exp = [DateTimeOffset]::UtcNow.AddHours(1).ToUnixTimeSeconds()
$payloadJson = '{"iss":"' + $issuer + '","aud":"' + $audience + '","email":"' + $Email + '","exp":' + $exp + '}'
$payload = ConvertTo-Base64Url ([Text.Encoding]::UTF8.GetBytes($payloadJson))

$hmac = [Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($secret))
$sig = ConvertTo-Base64Url ($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes("$header.$payload")))

"$header.$payload.$sig"
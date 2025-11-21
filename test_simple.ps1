Write-Host "Probando API de pedidos..."

# Obtener token
$response = Invoke-WebRequest -Uri "http://localhost:5000/api/Auth/login" -Method POST -Body '{"correo":"admin@cafeteria.com","contrasena":"Admin123!"}' -ContentType "application/json" -UseBasicParsing
$loginData = $response.Content | ConvertFrom-Json
$token = $loginData.token

Write-Host "Token obtenido: $($token.Substring(0,20))..."

# Crear pedido
$headers = @{"Authorization" = "Bearer $token"}
$body = '{"mesaId":1,"metodoPago":"Efectivo","esParaLlevar":false,"notas":"Test","articulos":[{"productoId":1,"cantidad":2}]}'

$pedidoResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/Pedidos" -Method POST -Body $body -ContentType "application/json" -Headers $headers -UseBasicParsing
$pedidoData = $pedidoResponse.Content | ConvertFrom-Json

Write-Host "Pedido creado:"
Write-Host "ID: $($pedidoData.id)"
Write-Host "Items count: $($pedidoData.articulos.Count)"
$pedidoData.articulos | ForEach-Object { Write-Host "- $($_.nombreProducto): $($_.cantidad)" }
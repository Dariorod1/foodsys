# Primero obtener un token de autenticación
$loginData = @{
    correo = "admin@cafeteria.com"
    contrasena = "Admin123!"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Auth/login" -Method POST -Body $loginData -ContentType "application/json"
$token = $loginResponse.token

Write-Host "Token obtenido: $($token.Substring(0,20))..."

# Crear el pedido
$pedidoData = Get-Content -Path "test_pedido.json" -Raw

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Write-Host "Creando pedido..."
$pedidoResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Pedidos" -Method POST -Body $pedidoData -Headers $headers

Write-Host "Pedido creado exitosamente:"
$pedidoResponse | ConvertTo-Json -Depth 10

Write-Host "`nVerificando items del pedido..."
Write-Host "Número de items: $($pedidoResponse.articulos.Count)"
foreach ($item in $pedidoResponse.articulos) {
    Write-Host "- $($item.nombreProducto): $($item.cantidad) x $($item.precioUnitario) = $($item.subtotal)"
}
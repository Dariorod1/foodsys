# Script para probar la creación de pedidos con items
Write-Host "=== PROBANDO CREACION DE PEDIDOS ==="

# Login para obtener token
$loginData = @{
    correo = "admin@cafeteria.com"
    contrasena = "Admin123!"
} | ConvertTo-Json

try {
    Write-Host "1. Obteniendo token de autenticación..."
    $loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Auth/login" -Method POST -Body $loginData -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "✓ Token obtenido correctamente"

    # Headers para las peticiones autenticadas
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }

    # Crear pedido con items
    $pedidoData = @{
        mesaId = 1
        metodoPago = "Efectivo"
        esParaLlevar = $false
        notas = "Pedido de prueba con items"
        articulos = @(
            @{
                productoId = 1
                cantidad = 2
            },
            @{
                productoId = 2
                cantidad = 1
            }
        )
    } | ConvertTo-Json -Depth 5

    Write-Host "2. Creando pedido con items..."
    $pedidoResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/Pedidos" -Method POST -Body $pedidoData -Headers $headers
    
    Write-Host "✓ Pedido creado exitosamente!"
    Write-Host "ID del pedido: $($pedidoResponse.id)"
    Write-Host "Monto total: $($pedidoResponse.montoTotal)"
    Write-Host "Cantidad de items: $($pedidoResponse.articulos.Count)"
    
    if ($pedidoResponse.articulos.Count -gt 0) {
        Write-Host "✓ ¡EXITO! Los items se guardaron correctamente:"
        foreach ($item in $pedidoResponse.articulos) {
            Write-Host "  - $($item.nombreProducto): $($item.cantidad) x $($item.precioUnitario) = $($item.subtotal)"
        }
    } else {
        Write-Host "✗ PROBLEMA: No se encontraron items en la respuesta"
    }

    # Verificar obteniendo el pedido individual
    Write-Host "`n3. Verificando pedido individual..."
    $pedidoIndividual = Invoke-RestMethod -Uri "http://localhost:5000/api/Pedidos/$($pedidoResponse.id)" -Method GET -Headers $headers
    Write-Host "Items en pedido individual: $($pedidoIndividual.articulos.Count)"
    
    if ($pedidoIndividual.articulos.Count -gt 0) {
        Write-Host "✓ ¡CONFIRMADO! Los items persisten en la base de datos"
    } else {
        Write-Host "✗ PROBLEMA CONFIRMADO: Los items no se guardaron en la base de datos"
    }

} catch {
    Write-Host "✗ ERROR: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        $responseText = $reader.ReadToEnd()
        Write-Host "Respuesta del servidor: $responseText"
    }
}
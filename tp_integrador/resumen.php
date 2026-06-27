<?php
session_start();

// 1. Seguridad: Restringir el acceso a usuarios no autenticados
if (!isset($_SESSION['documento'])) {
    header("Location: ingreso.html");
    exit();
}

$documento = $_SESSION['documento'];
$nombre = $_SESSION['nombre'];
$apellido = $_SESSION['apellido'];

// 2. Conexión a la base de datos
$conexion = new mysqli("db_UTN", "root", "abcde1234", "mi_banco_db", 3306);
if ($conexion->connect_error) {
    die("Error de conexión: " . $conexion->connect_error);
}

// 3. Buscar la tarjeta asociada al usuario
$stmt_tarjeta = $conexion->prepare("SELECT num_cuenta, numero_tarjeta, banco_emisor, saldo FROM tarjetas WHERE dni_titular = ?");
$stmt_tarjeta->bind_param("s", $documento);
$stmt_tarjeta->execute();
$res_tarjeta = $stmt_tarjeta->get_result();

$tiene_tarjeta = false;
if ($res_tarjeta->num_rows > 0) {
    $tiene_tarjeta = true;
    $tarjeta = $res_tarjeta->fetch_assoc();
    $num_cuenta = $tarjeta['num_cuenta'];

    // Buscar la última liquidación (ordenada por período descendente)
    $stmt_actual = $conexion->prepare("SELECT * FROM liquidaciones WHERE num_cuenta = ? ORDER BY periodo DESC LIMIT 1");
    $stmt_actual->bind_param("i", $num_cuenta);
    $stmt_actual->execute();
    $res_actual = $stmt_actual->get_result();
    $liquidacion_actual = $res_actual->fetch_assoc();

    // Buscar todo el historial de liquidaciones
    $stmt_historial = $conexion->prepare("SELECT * FROM liquidaciones WHERE num_cuenta = ? ORDER BY periodo DESC");
    $stmt_historial->bind_param("i", $num_cuenta);
    $stmt_historial->execute();
    $res_historial = $stmt_historial->get_result();
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mis Tarjetas - Panel</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>
<body class="bg-gray-100 font-sans min-h-screen flex flex-col">

    <header class="bg-[#004691] text-white py-4 shadow-md px-6 flex justify-between items-center">
        <h1 class="text-xl font-semibold">Mis <span class="font-bold">Tarjetas</span></h1>
        <div class="flex items-center gap-4">
            <span class="text-sm">Hola, <?php echo htmlspecialchars($nombre . ' ' . $apellido); ?></span>
            <a href="logout.php" class="bg-white text-[#004691] text-xs font-bold px-3 py-1 rounded hover:bg-gray-200 transition">Cerrar Sesión</a>
        </div>
    </header>

    <main class="flex-grow p-6 max-w-5xl mx-auto w-full mt-6">
        <?php if (!$tiene_tarjeta): ?>
            <div class="bg-white p-8 rounded shadow-md text-center border-l-4 border-yellow-500">
                <h2 class="text-xl text-gray-700">Aún no tenés tarjetas asociadas a tu cuenta.</h2>
                <p class="text-sm text-gray-500 mt-2">Acercate a una sucursal para solicitar la emisión de tu plástico.</p>
            </div>
        <?php else: ?>
            
            <div class="bg-gradient-to-r from-[#004691] to-blue-600 text-white p-6 rounded-lg shadow-lg mb-8 flex justify-between items-center">
                <div>
                    <p class="text-sm opacity-80 uppercase tracking-wider font-semibold"><?php echo htmlspecialchars($tarjeta['banco_emisor']); ?></p>
                    <p class="text-2xl font-mono tracking-widest mt-1">**** **** **** <?php echo substr($tarjeta['numero_tarjeta'], -4); ?></p>
                </div>
                <div class="text-right">
                    <p class="text-sm opacity-80 mb-1">Saldo Actual</p>
                    <p class="text-2xl font-bold">$<?php echo number_format($tarjeta['saldo'], 2, ',', '.'); ?></p>
                </div>
            </div>

            <?php if ($liquidacion_actual): ?>
            <h2 class="text-2xl font-bold text-gray-800 mb-4">Última Liquidación: <?php echo $liquidacion_actual['periodo']; ?></h2>
            <div class="bg-white rounded-lg shadow-md p-6 mb-10 flex flex-col md:flex-row justify-between items-center gap-4 border-l-4 border-green-500">
                <div>
                    <p class="text-gray-500 text-xs uppercase font-semibold tracking-wider">Vencimiento</p>
                    <p class="text-xl font-bold text-gray-800"><?php echo date("d/m/Y", strtotime($liquidacion_actual['fecha_vencimiento'])); ?></p>
                </div>
                <div>
                    <p class="text-gray-500 text-xs uppercase font-semibold tracking-wider">Pago Mínimo</p>
                    <p class="text-xl font-bold text-gray-800">$<?php echo number_format($liquidacion_actual['pago_minimo'], 2, ',', '.'); ?></p>
                </div>
                <div class="text-right">
                    <p class="text-gray-500 text-xs uppercase font-semibold tracking-wider">Total a Pagar</p>
                    <p class="text-4xl font-bold text-red-600">$<?php echo number_format($liquidacion_actual['total_a_pagar'], 2, ',', '.'); ?></p>
                </div>
            </div>
            <?php else: ?>
            <div class="bg-white p-4 rounded shadow-md mb-8 border-l-4 border-blue-400">
                <p class="text-gray-700">No hay liquidaciones emitidas para esta tarjeta.</p>
            </div>
            <?php endif; ?>

            <h3 class="text-xl font-bold text-gray-800 mb-4">Historial de Resúmenes</h3>
            <div class="bg-white rounded-lg shadow-md overflow-hidden">
                <table class="min-w-full leading-normal">
                    <thead>
                        <tr class="bg-gray-50 text-gray-600 text-left text-xs uppercase font-semibold">
                            <th class="px-5 py-3 border-b-2 border-gray-200">Período</th>
                            <th class="px-5 py-3 border-b-2 border-gray-200">Vencimiento</th>
                            <th class="px-5 py-3 border-b-2 border-gray-200">Pago Mínimo</th>
                            <th class="px-5 py-3 border-b-2 border-gray-200">Total Facturado</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php if ($res_historial->num_rows > 0): ?>
                            <?php while($fila = $res_historial->fetch_assoc()): ?>
                            <tr class="hover:bg-gray-50 transition">
                                <td class="px-5 py-4 border-b border-gray-200 text-sm">
                                    <p class="text-gray-900 font-bold"><?php echo $fila['periodo']; ?></p>
                                </td>
                                <td class="px-5 py-4 border-b border-gray-200 text-sm">
                                    <p class="text-gray-900"><?php echo date("d/m/Y", strtotime($fila['fecha_vencimiento'])); ?></p>
                                </td>
                                <td class="px-5 py-4 border-b border-gray-200 text-sm">
                                    <p class="text-gray-900">$<?php echo number_format($fila['pago_minimo'], 2, ',', '.'); ?></p>
                                </td>
                                <td class="px-5 py-4 border-b border-gray-200 text-sm">
                                    <p class="text-gray-900 font-semibold">$<?php echo number_format($fila['total_a_pagar'], 2, ',', '.'); ?></p>
                                </td>
                            </tr>
                            <?php endwhile; ?>
                        <?php else: ?>
                            <tr>
                                <td colspan="4" class="px-5 py-5 border-b border-gray-200 bg-white text-sm text-center text-gray-500">
                                    No hay registros históricos.
                                </td>
                            </tr>
                        <?php endif; ?>
                    </tbody>
                </table>
            </div>
        <?php endif; ?>
    </main>
</body>
</html>
<?php 
if(isset($stmt_tarjeta)) $stmt_tarjeta->close();
if(isset($stmt_actual)) $stmt_actual->close();
if(isset($stmt_historial)) $stmt_historial->close();
$conexion->close(); 
?>
<?php
// Iniciamos la sesión ANTES de cualquier salida HTML 
session_start();

// Conexión a la base de datos (credenciales del contenedor)
$conexion = new mysqli("db_UTN", "root", "abcde1234", "mi_banco_db", 3306);

if ($conexion->connect_error) {
    die("Error de conexión: " . $conexion->connect_error);
}

if (isset($_POST['ingresar'])) {
    $tipo_doc = $_POST['tipo_doc'];
    $documento = $_POST['documento'];
    $usuario = $_POST['usuario'];
    $password = $_POST['password'];

    // Consulta preparada para evitar inyección SQL al validar credenciales 
    $stmt = $conexion->prepare("SELECT * FROM usuarios WHERE documento = ? AND tipo_doc = ? AND usuario = ? AND password = ?");
    $stmt->bind_param("ssss", $documento, $tipo_doc, $usuario, $password);
    $stmt->execute();
    $resultado = $stmt->get_result();

    if ($resultado->num_rows === 1) {
        // Autenticación exitosa: Guardamos datos en la sesión 
        $fila = $resultado->fetch_assoc();
        $_SESSION['documento'] = $fila['documento'];
        $_SESSION['nombre'] = $fila['nombre'];
        $_SESSION['apellido'] = $fila['apellido'];

        // Redirigimos al panel del cliente
        header("Location: resumen.php");
        exit();
    } else {
        // Falla la autenticación: Volvemos al login con una alerta
        echo "<script>
                alert('Credenciales incorrectas. Verifique sus datos.');
                window.location.href = 'ingreso.html';
              </script>";
    }
    $stmt->close();
}
$conexion->close();
?>
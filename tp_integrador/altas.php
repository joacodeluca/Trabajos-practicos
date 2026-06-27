<?php
// Conexión a la base de datos
$conexion = new mysqli("db_UTN", "root", "abcde1234", "mi_banco_db", 3306);
if ($conexion->connect_error) {
    die("Error de conexión: " . $conexion->connect_error);
}

if (isset($_POST['registrar'])) {
    $tipo_doc = $_POST['tipo_doc'];
    $documento = $_POST['documento'];
    $nombre = $_POST['nombre'];
    $apellido = $_POST['apellido'];
    $fecha_nacimiento = $_POST['fecha_nacimiento'];
    $email = $_POST['email'];
    $usuario = $_POST['usuario'];
    $passwordA = $_POST['passwordA'];
    $passwordB = $_POST['passwordB'];

    // 1. Validar que las contraseñas ingresadas coincidan 
    if ($passwordA !== $passwordB) {
        die("<script>alert('Error: Las contraseñas no coinciden.'); window.location.href='registro.html';</script>");
    }

    // 2. Realizar la inserción en la tabla usuarios 
    $stmt = $conexion->prepare("INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password) VALUES (?, ?, ?, ?, ?, ?, ?, ?)");
    $stmt->bind_param("ssssssss", $documento, $tipo_doc, $nombre, $apellido, $fecha_nacimiento, $email, $usuario, $passwordA);

    if ($stmt->execute()) {
        // 3. Al crearse la cuenta, el sistema deberá asignar una tarjeta vinculada 
        // Generamos un número de 16 dígitos aleatorio
        $numero_tarjeta = "4512" . rand(1000, 9999) . rand(1000, 9999) . rand(1000, 9999);
        // Asignamos un banco por defecto permitido por el ENUM
        $banco_emisor = "Banco Nación";

        $stmt_tarjeta = $conexion->prepare("INSERT INTO tarjetas (numero_tarjeta, banco_emisor, dni_titular) VALUES (?, ?, ?)");
        $stmt_tarjeta->bind_param("sss", $numero_tarjeta, $banco_emisor, $documento);
        $stmt_tarjeta->execute();
        $stmt_tarjeta->close();

        echo "<script>
                alert('¡Cuenta creada exitosamente! Ya podés ingresar.');
                window.location.href = 'ingreso.html';
              </script>";
    } else {
        echo "<script>
                alert('Error al registrar: El documento, email o usuario ya están registrados en el sistema.');
                window.location.href = 'registro.html';
              </script>";
    }
    $stmt->close();
}
$conexion->close();
?>
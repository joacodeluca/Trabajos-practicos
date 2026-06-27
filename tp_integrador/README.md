# UTN FRH Tecnicatura Universitaria en P rogramación #
# Programación III 2026 #
# TP Integrador: Sistema de Consulta de Liquidaciones "Mis Tarjetas" 💳

¡Bienvenido al Trabajo Práctico Integrador de la asignatura **Programación III**! En este proyecto vas a trabajar en la integración de plataformas implementando un ecosistema donde una aplicación de escritorio y un portal web comparten la misma base de datos relacional.

---

## 📌 Objetivo del Trabajo Práctico
El objetivo es aplicar la POO al simular el circuito financiero real de la tarjeta **Progra3card**:
1. **Fase Administrativa (C# - Aplicación de Consola):** Los empleados de la entidad financiera emiten tarjetas registrando clientes y cargan los totales de las liquidaciones mensuales en la base de datos.
2. **Fase del Cliente (PHP - Portal Web):** El titular de la tarjeta realiza su activación digital (*onboarding*) validando su identidad, ingresa con sus credenciales y consulta su última liquidación, además de poder revisar el historial de resúmenes anteriores.

---

## 🗺️ Flujo de la Información e Integración
Para entender el orden de desarrollo y la interacción de las tecnologías, el flujo lógico del sistema sigue estos pasos:

1. **C# (Consola):** El personal de la tarjeta **Progra3card** registra a los nuevos clientes en la tabla `usuarios` (alta de un nuevo cliente) y emite una tarjeta registrando los datos de ésta en la tabla `tarjetas` (deberá seleccionar el banco emisor). Cada cliente es titular de una única tarjeta. A su vez carga mensualmente los resúmenes emitidos en la tabla `liquidaciones`.
2. **PHP (Web):** El usuario ingresa a `registro.html` e ingresa su DNI. El backend `altas.php` verifica que ya posea una tarjeta cargada por el banco y, de ser así, actualiza (`UPDATE`) su `usuario` y `password` (en texto plano) para activar su cuenta.
3. **PHP (Web):** El usuario inicia sesión en `ingreso.html` y accede a `resumen.php` para visualizar el estado de su cuenta.

---

## 🗄️ Estructura de la Base de Datos (`mi_banco_db`)
El motor de base de datos MySQL/MariaDB cuenta con el esquema ya provisto en este repositorio:
* **`usuarios`**: Almacena los datos personales del cliente. Los campos `usuario` y `password` inician en `NULL` hasta su activación web. Su clave primaria es el `documento`.
* **`tarjetas`**: Contiene la información del plástico (`numero_tarjeta`, `banco_emisor` como ENUM, `saldo` y `estado`). Tiene una relación estricta 1:1 vinculada mediante la clave foránea `dni_titular`.
* **`liquidaciones`**: Guarda las cabeceras financieras de los resúmenes (`periodo` en formato `YYYY-MM`, `fecha_vencimiento`, `total_a_pagar` y `pago_minimo`).

> 💡 **Nota:** El script `.sql` incluye registros de prueba (*seed data*) pre-cargados para que puedas probar el sistema inmediatamente.

---

## 📂 Archivos Base del Repositorio
En este repositorio vas a encontrar la estructura inicial para la pata web:
* 📄 `mi_banco_db.sql`: Script de creación de tablas y datos de prueba.
* 📄 `ingreso.html`: Formulario de login estilizado con Tailwind CSS.
* 📄 `registro.html`: Formulario de activación de usuario estilizado con Tailwind CSS.
* 📄 `Progra3card.cs`: Aplicación de consola C# (solo el "esqueleto") para la emisión, consulta, baja de tarjetas y emisión de liquidaciones.

### 🛠️ ¿Qué tenés que programar?
* **En PHP:** Deberás crear la lógica backend para el procesamiento del registro (`altas.php`), el inicio y control de sesiones (`ingreso.php`), y el panel interactivo del cliente (`resumen.php`) que realice los `JOIN` correspondientes para listar las liquidaciones.
* **En C#:** Deberás estructurar una solución de consola a partir del código provisto, que complete las funcionalidades requeridas, como ser conectarse a MySQL (usando MySQL Connector) y otras para dar de alta clientes/tarjetas y emitir nuevas liquidaciones.

---

## 🚫 Simplificaciones para esta Entrega
Para optimizar los tiempos de la cursada, quedan **excluidos** de este trabajo práctico:
* El desglose de consumos o compras individuales (el resumen se maneja solo a nivel de totales financieros).
* La encriptación o hasheo de claves (las contraseñas se almacenan y validan temporalmente en **texto plano**).
* Temas de infraestructura como contenedores Docker o entornos LEMP avanzados.

---
*¡Mucho éxito en el desarrollo! Ante cualquier duda, recordá consultar vía Teams o durante las clases presenciales.*

Osvaldo Cantone
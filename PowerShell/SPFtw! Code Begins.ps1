<# 
  Ejemplo de bloque de comentarios. Vamos a repasar algunos de los tipos de variables más utilizados:
       - Texto y números
       - Arrays
       - Hashtables
       - Tipos C#
       - Objetos personalizados
#>
 
function PrimeraFuncion {
	
	Write-Host "Mostrar mensaje tras invocar primera función"
}
 
function FuncionConPasoParametros {
	param (
	 [parameter(Position=1, Mandatory=$false)][string]$Saludo ="Bienvenido",
	 [parameter(Position=2, Mandatory=$true)][string]$Despedida
   	)
	$Mensaje = $Saludo + ". ", $Despedida
	Write-Host $Mensaje
}

try {
	# Escribir en la consola
	Write-Host "Mensaje de prueba"
	 
	# Esperar valor del usuario
	$RespuestaUsuario = Read-Host "Inserte un texto"

	Write-Host "Usted dijo $RespuestaUsuario"
	 
	# Variable texto
	$MiPrimeraVariable = "Hello World"
	 
	# Variable numérica
	$Numeros = 1
	 
	# Eliminar variables
	Remove-Variable -Name MiPrimeraVariable
	 
	# Crear variable global (a toda la ejecución de la consola)
	$Global:VariableGlobal = "valor global de muestra"
	 
	# Usar variable global
	Write-Host $VariableGlobal

	# Array (Creación)
	$Array = @()
	 
	# Array (Agregar elementos)
	$Array += "Primer elemento"
	$Array += "Segundo elemento"
	$Array += "Tercer elemento"
	$Array += "Cuarto elemento"
	 
	# Array (Mostrar elemento)
	$Array[0]
	 
	# Array (Eliminar elemento)
	$Array.Remove[0]
	 
	# Hashtable (Creación)
	$HashTable = @{"Clave1" = "Valor 1"; "Clave2" = "Valor 2"; "Clave3" = "Valor 3"}
	 
	# Hashtable (Agregar elemento)
	$HashTable.Add("Clave4", "Valor 4")
	 
	# Hashtable (Eliminar elemento)
	$HashTable.Remove("Clave2")
	 
	# Hashtable (Buscar)
	$HashTable.ContainsKey("Clave1") # Devolverá $true
	$HashTable.ContainsValue("7") # Devolverá $false

	 
	# Podemos llamar a funciones para ejecutar su código.
	PrimeraFuncion
	 
	# Si disponen de parámetros podemos pasarlos.
	FuncionConPasoParametros -Saludo "¡Hola!" -Despedida "Gracias por visitar SP ftw!"
	 
	# Si algún parámetro es opcional podemos llamar a la función sin indicarlo y se utilizará su valor por defecto.
	FuncionConPasoParametros -Despedida "Gracias por visitar SP ftw!"
}
catch {
	Write-Error "Ocurrió un error. Vaya a http://sharepointforthewin.com"
}

Write-Host "Gracias por confiar en SP ftw! Pulse enter y se cerrará."
pause
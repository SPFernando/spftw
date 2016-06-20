<# 
  Ejemplo de bloque de comentarios. Vamos a repasar algunos de los tipos de variables m�s utilizados:
       - Texto y n�meros
       - Arrays
       - Hashtables
       - Tipos C#
       - Objetos personalizados
#>

# Obtener la ruta dentro del sistema de ficheros en que se encuentra este fichero *.ps1
$Global:CurrentPath = Split-Path $MyInvocation.MyCommand.Path
# Invocar librer�a
Import-Module $CurrentPath"\SPFtw! Libreria.ps1"

try {
	# Escribir en la consola
	Write-Host "Mensaje de prueba"
	 
	# Esperar valor del usuario
	$RespuestaUsuario = Read-Host "Inserte un texto"

	Write-Host "Usted dijo $RespuestaUsuario"
	 
	# Variable texto
	$MiPrimeraVariable = "Hello World"
	 
	# Variable num�rica
	$Numeros = 1
	 
	# Eliminar variables
	Remove-Variable -Name MiPrimeraVariable
	 
	# Crear variable global (a toda la ejecuci�n de la consola)
	$Global:VariableGlobal = "valor global de muestra"
	 
	# Usar variable global
	Write-Host $VariableGlobal

	# Array (Creaci�n)
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
	 
	# Hashtable (Creaci�n)
	$HashTable = @{"Clave1" = "Valor 1"; "Clave2" = "Valor 2"; "Clave3" = "Valor 3"}
	 
	# Hashtable (Agregar elemento)
	$HashTable.Add("Clave4", "Valor 4")
	 
	# Hashtable (Eliminar elemento)
	$HashTable.Remove("Clave2")
	 
	# Hashtable (Buscar)
	$HashTable.ContainsKey("Clave1") # Devolver� $true
	$HashTable.ContainsValue("7") # Devolver� $false

	 
	# Podemos llamar a funciones para ejecutar su c�digo.
	PrimeraFuncion
	 
	# Si disponen de par�metros podemos pasarlos.
	FuncionConPasoParametros -Saludo "�Hola!" -Despedida "Gracias por visitar SP ftw!"
	 
	# Si alg�n par�metro es opcional podemos llamar a la funci�n sin indicarlo y se utilizar� su valor por defecto.
	FuncionConPasoParametros -Despedida "Gracias por visitar SP ftw!"
}
catch {
	Write-Error "Ocurri� un error. Vaya a http://sharepointforthewin.com"
}

Write-Host "Gracias por confiar en SP ftw! Pulse enter y se cerrar�."
pause

function PrimeraFuncion {
	
	Write-Host "Mostrar mensaje tras invocar primera funci�n"
}
 
function FuncionConPasoParametros {
	param (
	 [parameter(Position=1, Mandatory=$false)][string]$Saludo ="Bienvenido",
	 [parameter(Position=2, Mandatory=$true)][string]$Despedida
   	)
	$Mensaje = $Saludo + ". ", $Despedida
	Write-Host $Mensaje
}
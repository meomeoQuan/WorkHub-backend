$connectionString = "Server=localhost;Database=WorkHub;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
$userId = 4
$newDate = "2026-03-06 12:00:00"
$query = "UPDATE Users SET CreatedAt = '$newDate' WHERE Id = $userId"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $rows = $command.ExecuteNonQuery()
    Write-Host "Rows updated: $rows"
    $connection.Close()
} catch {
    Write-Error $_.Exception.Message
}

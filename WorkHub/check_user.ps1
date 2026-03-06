$connectionString = "Server=localhost;Database=WorkHub;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
$query = @"
    SELECT TOP 1 u.Id, u.FullName, u.CreatedAt, s.StartAt, s.[Plan] 
    FROM Users u 
    LEFT JOIN UserSubscriptions s ON u.Id = s.UserId 
    LEFT JOIN Post p ON u.Id = p.UserId 
    ORDER BY p.CreatedAt DESC
"@

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    $reader = $command.ExecuteReader()
    while($reader.Read()) {
        Write-Host "User_ID: $($reader['Id'])"
        Write-Host "FullName: $($reader['FullName'])"
        Write-Host "CreatedAt: $($reader['CreatedAt'])"
        Write-Host "StartAt: $($reader['StartAt'])"
        Write-Host "Plan: $($reader['Plan'])"
    }
    $connection.Close()
} catch {
    Write-Error $_.Exception.Message
}

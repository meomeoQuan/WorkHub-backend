$connectionString = "Server=localhost;Database=WorkHub;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
$userId = 4

$postQuery = "SELECT Id, CreatedAt FROM Post WHERE UserId = $userId ORDER BY CreatedAt DESC"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    $cmdPost = $connection.CreateCommand()
    $cmdPost.CommandText = $postQuery
    $reader = $cmdPost.ExecuteReader()
    
    Write-Host "Posts for User ID: $userId"
    while($reader.Read()) {
        Write-Host "Post ID: $($reader['Id']), CreatedAt: $($reader['CreatedAt'])"
    }
    
    $connection.Close()
} catch {
    Write-Error $_.Exception.Message
}

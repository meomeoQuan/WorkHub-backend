$connectionString = "Server=localhost;Database=WorkHub;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
$userId = 4

$postQuery = "SELECT COUNT(*) FROM Post WHERE UserId = $userId"
$applyQuery = "SELECT COUNT(*) FROM Application WHERE UserId = $userId"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    $cmdPost = $connection.CreateCommand()
    $cmdPost.CommandText = $postQuery
    $postCount = $cmdPost.ExecuteScalar()
    
    $cmdApply = $connection.CreateCommand()
    $cmdApply.CommandText = $applyQuery
    $applyCount = $cmdApply.ExecuteScalar()
    
    Write-Host "User ID: $userId"
    Write-Host "Post Count: $postCount"
    Write-Host "Application Count: $applyCount"
    
    $connection.Close()
} catch {
    Write-Error $_.Exception.Message
}

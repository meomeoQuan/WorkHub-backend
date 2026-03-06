$connectionString = "Server=localhost;Database=WorkHub;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
$userId = 4
# Set CreatedAt to 1 month ago to verify limit can be reached, then moved to 'new month'
$pastDate = "2026-02-01 00:00:00"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    # 1. Delete all posts to start fresh
    $delRecs = "DELETE FROM Recruitment WHERE UserId = $userId"
    $delPosts = "DELETE FROM Post WHERE UserId = $userId"
    
    $cmdDelRecs = $connection.CreateCommand()
    $cmdDelRecs.CommandText = $delRecs
    $cmdDelRecs.ExecuteNonQuery()
    
    $cmdDelPosts = $connection.CreateCommand()
    $cmdDelPosts.CommandText = $delPosts
    $cmdDelPosts.ExecuteNonQuery()
    
    # 2. Update user CreatedAt to a fixed past date
    $updateUser = "UPDATE Users SET CreatedAt = '$pastDate' WHERE Id = $userId"
    $cmdUpdate = $connection.CreateCommand()
    $cmdUpdate.CommandText = $updateUser
    $cmdUpdate.ExecuteNonQuery()
    
    Write-Host "User 4 reset successfully. Posts deleted. CreatedAt set to $pastDate"
    $connection.Close()
} catch {
    Write-Error $_.Exception.Message
}

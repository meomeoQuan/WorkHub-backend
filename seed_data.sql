UPDATE Recruitment 
SET Benefits = 'Flexible Hours' + CHAR(10) + 'Health Insurance' + CHAR(10) + 'Competitive Pay' 
WHERE JobName = 'Junior .NET Developer';

UPDATE Recruitment 
SET Benefits = 'Remote Work' + CHAR(10) + 'Stock Options' + CHAR(10) + 'Career Growth' 
WHERE JobName = 'Frontend Developer';

UPDATE UserDetail 
SET Description = 'Platform Administrator with expertise in system security and user management.' 
WHERE FullName = 'Admin';

UPDATE UserDetail 
SET Description = 'Passionate full-stack developer with a focus on .NET and React technologies.' 
WHERE FullName = 'User One';

UPDATE UserDetail 
SET Description = 'UI/UX Designer who loves creating beautiful and intuitive user interfaces.' 
WHERE FullName = 'User Two';
GO

IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ReservationView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[ReservationView]
AS
SELECT       NEWID() AS ReservationId, dbo.Movie.Title, dbo.Movie.ReleaseDate, dbo.Actor.FirstName + '' '' + dbo.Actor.LastName AS Actor, dbo.Actor.DateOfBirth
FROM            dbo.Actor INNER JOIN
                         dbo.Movie ON dbo.Actor.Id = dbo.Movie.Actor_Id
'



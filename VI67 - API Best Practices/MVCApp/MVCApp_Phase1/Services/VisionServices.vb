Public Class VisionServices
    Public Function GetSystemInfo()
        'create a soap client instance
        Dim _service As New VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient

        'provide the database as seen in the dropdown box of the login screen. 
        '!!!Do not use the actual database name in the SQL Server!!!
        Dim _databaseName As String = "VisionDemo76 (SQLDB)"

        'a user name as defined in Vision's Security\Users configuration
        Dim _username As String = "ADMIN"

        'the password assigned to the user
        Dim _password As String = ""

        'Tip: using XElement variables let you write standard XML data in code without
        'having to use strings and formatting (VB.net only)
        Dim _connectionInfo As XElement = <VisionConnInfo>
                                              <databaseDescription><%= _databaseName %></databaseDescription>
                                              <userName><%= _username %></userName>
                                              <userPassword><%= _password %></userPassword>
                                          </VisionConnInfo>

        'request the system info from your vision database
        Dim _retval = _service.GetSystemInfo(_connectionInfo.ToString())

        'return this to the calling method
        Return _retval
    End Function
End Class

Public Class VisionServices

    Private _connectionInfo As ConnectionInfo
    Public Property ConnectionInfo() As ConnectionInfo
        Get
            Return _connectionInfo
        End Get
        Set(ByVal value As ConnectionInfo)
            _connectionInfo = value
        End Set
    End Property

    Public Sub New()

    End Sub

    Public Sub New(connectionInfo As ConnectionInfo)
        _connectionInfo = connectionInfo
    End Sub

    ''' <summary>
    ''' returns the correct connection string for database and user info
    ''' </summary>
    ''' <param name="database"></param>
    ''' <param name="user"></param>
    ''' <param name="password"></param>
    ''' <returns></returns>
    Public Function GetConnectionInfo(database As String, user As String, password As String)
        Dim _connectionInfo As XElement = <VisionConnInfo>
                                              <databaseDescription><%= database %></databaseDescription>
                                              <userName><%= user %></userName>
                                              <userPassword><%= password %></userPassword>
                                          </VisionConnInfo>

        Return _connectionInfo.ToString()
    End Function

    ''' <summary>
    ''' connection string with an already established session id
    ''' </summary>
    ''' <param name="sessionId"></param>
    ''' <returns></returns>
    Public Function GetConnectionInfo(sessionId As String)
        Dim _connectionInfo As XElement = <VisionConnInfo>
                                              <sessionId><%= sessionId %></sessionId>
                                          </VisionConnInfo>
        Return _connectionInfo.ToString()
    End Function


    Public Function GetProjectByID(id As String) As ProjectInfo
        Dim _retval = New ProjectInfo
        Dim _service As New VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient
        Dim _connInfoXML As String = GetConnectionInfo(
            _connectionInfo.DatabaseName,
            _connectionInfo.UserName,
            _connectionInfo.Password)
        'this is a peculiarity in the getbyid for projects. The project key is ALWAYS for WBS1/WBS2/WBS3
        'so we need to pass it as this 3 level key with subkeys
        Dim _keys As XElement = <KeyValues>
                                    <Keys ID="1">
                                        <Key>
                                            <Fld><%= id %></Fld>
                                            <Fld><%= " " %></Fld>
                                            <Fld><%= " " %></Fld>
                                        </Key>
                                    </Keys>
                                </KeyValues>
        'retrieve data for project info
        Dim _xmlmessage = _service.GetProjectsByKey(_connInfoXML, _keys.ToString, "Primary")
        'turn into a xml document for easier management
        Dim _xDoc = XDocument.Load(New System.IO.StringReader(_xmlmessage))

        'go to the first record in the returned value
        Dim _project = _xDoc.Element("RECS").Elements("REC").FirstOrDefault()
        If _project IsNot Nothing Then
            _retval.WBS1 = _project.Element("PR").Element("ROW").Element("WBS1")
            _retval.Name = _project.Element("PR").Element("ROW").Element("Name")
            _retval.LongName = _project.Element("PR").Element("ROW").Element("LongName")
            _retval.SubLevel = _project.Element("PR").Element("ROW").Element("SubLevel")
        End If
        Return _retval
    End Function



    Public Function GetProjectByQuery(sqlStatement As String) As IList(Of ProjectInfo)
        Dim _retval = New List(Of ProjectInfo)
        Dim _service As New VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient
        Dim _connInfoXML As String = GetConnectionInfo(
            _connectionInfo.DatabaseName,
            _connectionInfo.UserName,
            _connectionInfo.Password)
        'retrieve data for project info
        Dim _xmlmessage = _service.GetProjectsByQuery(_connInfoXML, sqlStatement, "Primary")
        'turn into a xml document for easier management
        Dim _xDoc = XDocument.Load(New System.IO.StringReader(_xmlmessage))

        'go to the first record in the returned value
        For Each e As XElement In _xDoc.Element("RECS").Elements("REC")
            If e IsNot Nothing Then
                Dim _pr As New ProjectInfo
                _pr.WBS1 = e.Element("PR").Element("ROW").Element("WBS1")
                _pr.Name = e.Element("PR").Element("ROW").Element("Name")
                _pr.LongName = e.Element("PR").Element("ROW").Element("LongName")
                _pr.SubLevel = e.Element("PR").Element("ROW").Element("SubLevel")
                _retval.Add(_pr)
            End If
        Next
        Return _retval
    End Function

    Public Function SaveProjectInfo(project As ProjectInfo)
        Dim _service As New VisionAPI.DeltekVisionOpenAPIWebServiceSoapClient
        Dim _xmlmessage As String
        Dim _connInfoXML As String = GetConnectionInfo(
            _connectionInfo.DatabaseName,
            _connectionInfo.UserName,
            _connectionInfo.Password)

        Dim _xDoc As XDocument = New XDocument(
            <RECS>
                <REC>
                    <PR name="PR" alias="PR" keys="WBS1,WBS2,WBS3">
                        <ROW tranType="UPDATE">
                            <WBS1><%= project.WBS1 %></WBS1>
                            <WBS2><%= " " %></WBS2>
                            <WBS3><%= " " %></WBS3>
                            <Name><%= project.Name %></Name>
                            <SubLevel><%= project.SubLevel %></SubLevel>
                            <LongName><%= project.LongName %></LongName>
                        </ROW>
                    </PR>
                </REC>
            </RECS>
        )

        'apply the default vision namespace
        _xDoc = ApplyDefaultNamespace(_xDoc)

        _xmlmessage = _service.UpdateProject(_connInfoXML, _xDoc.ToString())

        If _xmlmessage.Contains("error") Then
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' applies the default vision namespace to a XDocument
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <returns></returns>
    Private Function ApplyDefaultNamespace(doc As XDocument) As XDocument
        Dim _xn As XNamespace = "http://deltek.vision.com/XMLSchema"
        For Each e As XElement In doc.Descendants()
            e.Name = _xn + e.Name.LocalName
        Next

        Return doc
    End Function
End Class

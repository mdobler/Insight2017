Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports MVCApp_Phase3

<TestClass()> Public Class ServiceTests
    Dim _databaseName As String = "VisionDemo76 (SQLDB)"
    Dim _username As String = "ADMIN"
    Dim _password As String = ""

    <TestMethod()> Public Sub GetProjectByQueryTest()
        Dim _conn As New ConnectionInfo() With {.DatabaseName = _databaseName, .UserName = _username, .Password = _password}
        Dim _vservices = New VisionServices(_conn)

        Dim _retval = _vservices.GetProjectByQuery("name like 'adelphi%'")

        Assert.IsNotNull(_retval)
        Assert.IsTrue(_retval.Count > 0)
    End Sub

    <TestMethod()> Public Sub GetProjectByIdTest()
        Dim _conn As New ConnectionInfo() With {.DatabaseName = _databaseName, .UserName = _username, .Password = _password}
        Dim _vservices = New VisionServices(_conn)

        Dim _retval = _vservices.GetProjectByID("2003005.00")

        Assert.IsNotNull(_retval)
        Assert.IsTrue(_retval.WBS1 = "2003005.00")
    End Sub

    <TestMethod()> Public Sub SaveProjectInfoTest()
        Dim _conn As New ConnectionInfo() With {.DatabaseName = _databaseName, .UserName = _username, .Password = _password}
        Dim _vservices = New VisionServices(_conn)

        Dim _retval = _vservices.GetProjectByID("2003005.00")

        _retval.LongName += "XXXXX"
        Dim _saveok = _vservices.SaveProjectInfo(_retval)

        Assert.IsTrue(_saveok)
    End Sub
End Class
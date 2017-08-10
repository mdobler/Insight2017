Imports System.Web.Mvc

Namespace Controllers
    Public Class ProjectsController
        Inherits Controller

        'initialize service
        Private _service As VisionServices

        Public Sub New()
            _service = New VisionServices(New ConnectionInfo() With {
                .DatabaseName = My.Settings.DatabaseName,
                .UserName = My.Settings.UserName,
                .Password = My.Settings.Password
            })
        End Sub

        ' GET: Projects
        Public Function Index() As ActionResult
            Dim _query = "select * from PR Where PR.WBS1 in " +
                "(select top 50 WBS1 from PR Where ChargeType = 'R' order by createdate desc)"
            Dim _result = _service.GetProjectByQuery(_query)
            Return View(_result)
        End Function

        ' GET: Projects/Details/5
        Public Function Details(ByVal wbs1 As String) As ActionResult
            Dim _result = _service.GetProjectByID(wbs1)
            Return View(_result)
        End Function
    End Class
End Namespace
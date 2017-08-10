Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class UnitTest1

    <TestMethod()> Public Sub TestMethod1()
        Dim _vservices = New MVCApp_Phase1.VisionServices()
        Dim _retval = _vservices.GetSystemInfo

        Assert.IsFalse(String.IsNullOrEmpty(_retval))
    End Sub

End Class
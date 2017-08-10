@Code
    ViewData("Title") = "Home Page"
End Code

<div class="jumbotron">
    <h1>VI-67: Deltek Vision API Sample</h1>
    <p class="lead">This sample shows how to query data from Vision via the API to retrieve lists and details (proects)</p>
</div>

<div class="row">
    <div class="col-md-4">
        <h2>Getting started</h2>
        <p>
            Click here to get to a list of new projects in the database
        </p>
        <p>@Html.ActionLink("Projects", "Index", "Projects")</p>
    </div>
    <div class="col-md-4">
        <h2></h2>
        <p></p>
    </div>
    <div class="col-md-4">
        <h2> </h2>
        <p></p>
    </div>
</div>

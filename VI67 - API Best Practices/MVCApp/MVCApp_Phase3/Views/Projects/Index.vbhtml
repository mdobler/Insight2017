@ModelType IEnumerable(Of MVCApp_Phase3.ProjectInfo)
@Code
ViewData("Title") = "Index"
End Code

<h2>List Of New Projects</h2>

<p>
    @Html.ActionLink("Create New", "Create")
</p>
<table class="table">
    <tr>
        <th>
            @Html.DisplayNameFor(Function(model) model.WBS1)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.Name)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.LongName)
        </th>
        <th></th>
    </tr>

@For Each item In Model
    @<tr>
        <td>
            @Html.DisplayFor(Function(modelItem) item.WBS1)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.Name)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.LongName)
        </td>
        <td>@Html.ActionLink("Details", "Details", New With {.wbs1 = item.WBS1}) |
            @Html.ActionLink("Edit", "Edit", New With {.wbs1 = item.WBS1}) 
        </td>
    </tr>
Next

</table>

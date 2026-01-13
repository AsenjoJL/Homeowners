# Quick View Template Guide

## Standard View Structure

All views follow this pattern:

```html
@model [ModelType]

@{
    ViewData["Title"] = "Page Title";
}

<div class="container-fluid">
    <h2><i class="fas fa-icon me-2"></i>Page Title</h2>
    
    <!-- Content -->
    
    <!-- Modals (if needed) -->
    
    <!-- Scripts -->
    <script>
        // AJAX functions
    </script>
</div>
```

## Common Patterns

### List View
- Table with Bootstrap classes
- Action buttons (Edit, Delete)
- AJAX delete functions

### Form View
- Bootstrap form controls
- AJAX submit function
- Success/error handling

### Modal Forms
- Bootstrap modal
- Form inside modal
- AJAX submit
- Reload on success

## AJAX Pattern

```javascript
$.ajax({
    url: '@Url.Action("Action", "Controller")',
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify(data),
    success: function(response) {
        if (response.success) {
            alert('Success!');
            location.reload();
        } else {
            alert('Error: ' + response.message);
        }
    }
});
```

## Controller Response Pattern

```csharp
return Json(new { success = true, message = "Success message!" });
// or
return Json(new { success = false, message = "Error message" });
```

Use this template to create remaining views quickly!


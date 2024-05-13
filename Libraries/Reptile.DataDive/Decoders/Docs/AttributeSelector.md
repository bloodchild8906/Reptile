# AttributeSelector Documentation

The `AttributeSelector` class is used to select HTML elements based on specific attributes. It supports multiple modes
of matching, including exact matches, regular expression matches, contains checks, and existence checks.

## Constructor

The constructor initializes a new instance of the `AttributeSelector` with the specified attribute name, optional value,
and case sensitivity.

### Syntax

```csharp
public AttributeSelector(string name, string? value = null, bool ignoreCase = true)
```

### Parameters

- **name**: The name of the attribute to match against.
- **value**: The value of the attribute to match against. This parameter is optional.
- **ignoreCase**: Specifies whether the attribute value comparison should be case-insensitive.

### Example

```csharp
var selector = new AttributeSelector("class", "active", true);
```

## Properties

### Mode

Sets or gets the mode of comparison used for matching HTML elements.

#### Syntax

```csharp
public AttributeSelectorMode Mode { get; set; }
```

#### Example

```csharp
selector.Mode = AttributeSelectorMode.Contains;
```

## Methods

### MatchComparer

Determines if an HTML element matches based on an exact match of the attribute value.

#### Syntax

```csharp
private bool MatchComparer(HtmlElementNode node)
```

#### Example

```csharp
var isMatch = selector.IsMatch(someHtmlElementNode);
```

### RegExComparer

Determines if an HTML element's attribute value matches a specified regular expression.

#### Syntax

```csharp
private bool RegExComparer(HtmlElementNode node)
```

#### Example

```csharp
selector.Mode = AttributeSelectorMode.RegEx;
var isMatch = selector.IsMatch(someHtmlElementNode);
```

### ContainsComparer

Checks if any word within an HTML element's attribute value matches the specified value exactly.

#### Syntax

```csharp
private bool ContainsComparer(HtmlElementNode node)
```

#### Example

```csharp
selector.Mode = AttributeSelectorMode.Contains;
var isMatch = selector.IsMatch(someHtmlElementNode);
```

### ExistsOnlyComparer

Determines if an HTML element has a specified attribute, regardless of its value.

#### Syntax

```csharp
private bool ExistsOnlyComparer(HtmlElementNode node)
```

#### Example

```csharp
selector.Mode = AttributeSelectorMode.ExistsOnly;
var hasAttribute = selector.IsMatch(someHtmlElementNode);
```

### ToString

Returns a string that represents the current `AttributeSelector`.

#### Syntax

```csharp
public override string ToString()
```

#### Example

```csharp
string description = selector.ToString();
```
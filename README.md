# Frends.Kungsbacka.Json

This is a replacement for Frends.Json.

## New and changed tasks

### All tasks

Frends.Json has a method for converting input to JToken. Currently, it supports string and JToken.
Frends.Kungsbacka.Json changes this slightly to support all objects where `ToString()` returns
the object serialized as JSON. This includes `CaseInsensitivePropertyTree` which is the object
Frends use to deserialize data coming from another process (i.g. invoking a subprocess).
`CaseInsensitivePropertyTree` is also used for trigger parameters. This removes the need to
manually deserialize data passed between processes before passing it to a JSON task.

### Validate task

Validate task has switched from using [Json.NET Schema](https://www.newtonsoft.com/jsonschema)
to [NJsonSchema](https://github.com/RicoSuter/NJsonSchema).

### Handlebars task

Handlebars now support using [[square brackets]] instead of {{curly braces}} in Handlebars templates
and partials. The square brackets get replaced with curly braces before the template is passed to
Handlebars. When using square brackets the template or partial no longer has to be an expression with
a verbatim string (@"template") but can be text instead. This opens up the possibility to use Frends
expression syntax directly inside templates without adding an extra task to create the template. The
feature relies on regex with balanced groups and does not use a full parser. It supports escaping, but
there will likely be corner cases that will fail.

[Handlebars.Net](https://github.com/Handlebars-Net/Handlebars.Net) supports adding custom helper
functions. This is now exposed in the Handlebars task. Custom helpers can be declared inside a C#
statement and assigned to a variable that can then be referenced in a Handlebars task, or you can
create a helper directly on the task.

### New Map task

Introducing a new task called Map that can create a new `JObject` by querying an existing `JObject`.
It can handle defaults if a property does not exist and do simple transformations. Map also supports
custom transformations that are similar to the Handlebars tasks helper functions.

### New ConvertXmlBytesToJToken task

ConvertXmlStringToJToken has got a new sibling task called ConvertXmlBytesToJToken. It's
useful when you can't know the XML encoding without parsing the XML declaration. Using this
task you don't have to convert the XML content to a string before converting it to JSON.
The task uses `System.Xml.XmlDocument` to figure out the encoding.

### Query, QuerySingle, ConvertJsonStringToJToken, and ConvertXmlStringToJToken

Query, QuerySingle, ConvertJsonStringToJToken, and ConvertXmlStringToJToken should all work as
they do in Frends.Json with the addition of being able to deserialize more types of input
([see "All tasks" above](#all-tasks)).

## Documentation

This readme file only contains detailed documentation about new and changed tasks. For tasks
that have not had any functional changes, you can use the official documentation for
[Frends.Json](https://github.com/Kungsbacka/Frends.Json).

### Square brackets in Handlebars templates and partials

Handlebars use {{curley braces}} for expressions in a template. Since Frends also use Handlebars 
notation for mixing code elements with text, XML, JSON, etc, you have to use expression mode
with a verbatim string (@"") when you create Handlebars templates to not confuse Frends.
This makes it impossible to mix in Frends code elements in Handlebars templates. One way around
this problem is to create the template in an expression block before the Handlebars task.

By switching to [[square brackets]] for Handlebars you can now freely mix Handlebars expressions
and code elements with curly braces directly in the Handlebars task. Just change from expression
mode to text, XML, or JSON and remove the verbatim string.

Handlebars.Net does not support square brackets and there is no way to tell Handlebars.Net to
use square brackets instead of curly braces. Before the template is sent to Handlebars for
compilation, square brackets are replaced with curly braces. This is done using regular
expressions with balanced groups and a little bit of extra parsing. Handlebars.Net uses a
parser and not regex, so don't expect square brackets to behave the same as using curly braces
directly. But it will work fine for most cases.

The square brackets feature has to be enabled under Options. Here is an example of a template
that uses square brackets.

```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
<Person>
    <FullName>[[firstname]] [[lastname]]</FullName>
    <Created>{{DateTime.Today.ToString("yyyy-MM-dd")}}</Created>
    <Source>{{#var.source}}</Source>
</Person>
```

Before it's sent to Handlebars, Frends will process all code elements and after that, all angle
brackets are replaced by curly braces. The resulting template will look something like this:


```xml
<?xml version="1.0" encoding="UTF-8" standalone="yes" ?>
<Person>
    <FullName>{{firstname}} {{lastname}}</FullName>
    <Created>2021-12-01</Created>
    <Source>Active Directory</Source>
</Person>
```

### Custom Helpers for Handlebars

Handlebars.Net supports custom helpers and this functionality is now exposed in the Handlebars
task. The way you define a custom helper is by creating a delegate of one of the following types:
`Action<System.IO.TextWriter,dynamic,object[]>` or if its a block helper
`Action<System.IO.TextWriter,dynamic,dynamic,object[]>`.
`System.IO.TextWriter` must include the namespace since `System.IO` is not one of the namespaces
included in a process.

Below are templates for the two different kinds of helpers that you can use as a starting point.

```C#
new Action<System.IO.TextWriter, dynamic, object[]>((writer, context, arguments) =>
{
    // Use writer to output data
    // Context contains JSON object
    // arguments contain the arguments supplied when calling the helper
})
```

```C#
new Action<System.IO.TextWriter, dynamic, dynamic, object[]>((writer, options, context, arguments) =>
{
})
```

### Map

The purpose of Map is to take one JObject and convert it to another JObject. It supports
transformations (both custom and Built-in) and default values.

A map can look something like this:

```JSON
[
    {"from": "firstname", "to": "givenname"},
    {"from": "lastname", "to": "surname"}
]
```

This will output a new JObject where *firstname* from the source object is mapped to *givenname*
in the target object and *lastname* is mapped to *surname*. If no target object is supplied, a new
JObject will be created.

#### SelectToken

JToken har a `SelectToken` method that can be used to query a JObject
([more details here](https://www.newtonsoft.com/json/help/html/SelectToken.htm)). To use
`SelectToken` to select what is mapped, you prefix the source name with a question mark (?).
The example below uses JSONPath syntax.

```JSON
[
    {"from": "firstname", "to": "givenname"},
    {"from": "lastname", "to": "surname"},
    {"from": "?$.addresses[0].zipCode", "to": "zipCode"}
]
```

If a source name starts with a question mark, you double up to avoid using SelectToken. In the
example below the name `"??optional"` becomes `"?optional"` and that name is used when selecting
the property, not SelectToken.

```JSON
[
    {"from": "firstname", "to": "givenname"},
    {"from": "lastname", "to": "surname"},
    {"from": "??optional", "to": "optional"}
]
```

#### Multiple from properties

From can take an array of property names and/or SelectToken/JSONPath expressions. Map will try the names and
expressions in order and the first non-null value is copied to the target property.

```JSON
[
    {"from": ["firstname", "first_name", "?$.persons[0].name"], "to": "givenname"},
    {"from": "lastname", "to": "surname"},
    {"from": "??optional", "to": "optional"}
]
```

#### Default value

Map supports default values that are used only if a property does not exist at all or if the
property exists and the value is null. A map with a default value can look like the example below.

```JSON
[
    {"from": "firstname", "to": "givenname"},
    {"from": "lastname", "to": "surname"},
    {"from": "?$.addresses[0].zipCode", "to": "zipCode"},
    {"from": "role", "to": "role", "def": "User"}
]
```

Note that *from* and *to* map to the same name. This is not required when using default values,
but it shows a common use case for default values where you just want to give an existing property
a default value without renaming it.

#### Do not overwrite

If an existing object is supplied as the target object, you can tell Map not to overwrite the
target property value if it already exists. You do this by adding an exclamation mark (!) to the
end of the target property name.

```JSON
[
    {"from": "firstname", "to": "givenname"},
    {"from": "lastname", "to": "surname"},
    {"from": "?$.addresses[0].zipCode", "to": "zipCode"},
    {"from": "role", "to": "role", "def": "User"},
    {"from": "status", "to": "user_status!"}
]
```

If *user_status* already exists in the target object, it will not be overwritten by the
value of *status* in the source object.

As with `SelectToken` above, you can double up if you want to escape the exclamation mark.

#### Transformations

Map can transform a value before it is added to the target object. There are both Built-in
transformations and support for adding custom transformations.

The built-in transformations currently available are: *LCase*, *UCase*, *Trim*, *SweSsn* (format as
Swedish personnummer ("SSN")), and *SweOrgNr* (format as Swedish organization number (organisationsnummer)). 

The example below uses the LCase transformation to make the value lowercase.

```JSON
[
    {"from": "firstname", "to": "givenname"},
    {"from": "lastname", "to": "surname"},
    {"from": "?$.addresses[0].zipCode", "to": "zipCode"},
    {"from": "role", "to": "role", "def": "User"},
    {"from": "status", "to": "user_status*"},
    {"from": "lang", "to": "language", "trans": "LCase"}
]
```

Map also supports custom transformations supplied in the task optional parameters.
The transformation function is a delegate of type `Func<JToken, JToken>` (takes a `JToken` and returns a `JToken`).

Below is an example transformation that joins all elements in a `JArray` and returns a new `string` value.

```C#
new Func<JToken, JToken>((input) =>
{
    if (input is JArray array)
    {
        return string.Join(";", array);
    }
    return input;
});
```

### ConvertXmlBytesToJToken

ConvertXmlBytesToJToken does the same thing as ConvertXmlStringToJToken but takes a byte array
instead of a string as input. The byte array is used to construct a `System.Xml.XmlDocument`
that is then serialized to Json with `JsonConvert.SerializeXmlNode`. This is the same thing
ConvertXmlStringToJToken does, but with a string. This is useful in scenarios where you don't
know how the XML is encoded without looking at the XML declaration (when you get an HTTP
content&#8209;type without charset for example). This way you skip the steps of figuring out the encoding and
converting the XML to a string before passing it to the converter.

### SanitizeCDataSections

SanitizeCDataSections is a helper function that can be used to remove #cdata-section sections from a JToken.
When converting from xml to json, #cdata-section sections are included in the json output. 
The task takes a JToken as input and returns a JToken with all #cdata-section sections removed. This can be useful
when you've converted an XML to JSON but want to be rid of the #cdata-section sections in the JSON.

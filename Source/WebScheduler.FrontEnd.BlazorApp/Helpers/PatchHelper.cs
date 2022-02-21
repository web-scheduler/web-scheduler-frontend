namespace WebScheduler.FrontEnd.BlazorApp.Helpers;
#nullable disable
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;

/// <summary>
/// this a general tool to generate JsonPatchDocument by comparing two objects recursively
/// See https://gist.github.com/yww325/b71563462cb5b5f2ea29e0143634bebe
/// and https://stackoverflow.com/a/50011301/103302
/// </summary>
public static class PatchHelper
{
    private const char PathDelimiter = '/';
    private const char Dash = '-';
    public static JsonPatchDocument CompareObjects(object oldObj, object newObj, string path = "/")
    {
        var original = JObject.FromObject(oldObj);
        var modified = JObject.FromObject(newObj);
        var patch = new JsonPatchDocument();
        FillPatchForObject(original, modified, patch, path);
        return patch;
    }

    private static void FillPatchForObject(JObject orig, JObject mod, JsonPatchDocument patch, string path)
    {
        var origNames = orig.Properties().Select(x => x.Name).ToArray();
        var modNames = mod.Properties().Select(x => x.Name).ToArray();

        PatchRemovedProperty(patch, origNames, modNames, path);
        PatchAddedProperty(patch, origNames, modNames, path, mod);

        // Present in both
        foreach (var k in origNames.Intersect(modNames))
        {
            var origProp = orig.Property(k);
            var modProp = mod.Property(k);

            if (origProp.Value.Type != modProp.Value.Type)
            {
                // even type is changed, like from 123 to "hello"
                _ = patch.Replace(path + modProp.Name, modProp.Value.ToObject<object>());
            }
            else if (!string.Equals(
                origProp.Value.ToString(Newtonsoft.Json.Formatting.None),
                modProp.Value.ToString(Newtonsoft.Json.Formatting.None), StringComparison.Ordinal))
            {
                // type is the same, just value changed
                PatchPropertyValueChange(patch, origProp, modProp, path);
            }
        }
    }

    private static void PatchPropertyValueChange(JsonPatchDocument patch, JProperty origProp, JProperty modProp, string path)
    {
        if (origProp.Value.Type == JTokenType.Object)
        {
            // Recurse into objects
            FillPatchForObject(origProp.Value as JObject, modProp.Value as JObject, patch, path + modProp.Name + PathDelimiter);
        }
        else if (origProp.Value.Type == JTokenType.Array)
        {
            var oldArray = origProp.Value as JArray;
            var newArray = modProp.Value as JArray;
            PatchArray(patch, oldArray, newArray, path + modProp.Name);
        }
        else
        {
            // Replace values directly for simple type ,like 123 to 456
            _ = patch.Replace(path + modProp.Name, modProp.Value.ToObject<object>());
        }
    }

    private static void PatchArray(JsonPatchDocument patch, JArray oldArray, JArray newArray, string path)
    {
        for (var i = 0; i < oldArray.Count; i++)
        {
            if (newArray.Count - 1 < i)
            {
                _ = patch.Remove(path + PathDelimiter + i);
            }
            else if (oldArray[i] is JObject && newArray[i] is JObject)
            {
                // Recurse into array element
                FillPatchForObject(oldArray[i] as JObject, newArray[i] as JObject, patch, path + PathDelimiter + i + PathDelimiter);
            }
            else
            {
                //direct compare
                if (!JToken.DeepEquals(oldArray[i], newArray[i]))
                {
                    _ = patch.Replace(path + PathDelimiter + i, newArray[i]);
                }
            }
        }

        for (var i = oldArray.Count; i < newArray.Count; i++)
        {
            _ = patch.Add(path + PathDelimiter + Dash, newArray[i]);
        }
    }

    private static void PatchAddedProperty(JsonPatchDocument patch, string[] origNames, string[] modNames, string path, JObject mod)
    {
        // Names added in modified
        foreach (var k in modNames.Except(origNames))
        {
            var prop = mod.Property(k);
            _ = patch.Add(path + prop.Name, prop.Value.ToObject<object>()); // the value will be serialized to json anyway.
        }
    }

    private static void PatchRemovedProperty(JsonPatchDocument patch, string[] origNames, string[] modNames, string path)
    {
        // Names removed in modified
        foreach (var k in origNames.Except(modNames))
        {
            _ = patch.Remove(path + k);
        }
    }
}

local tagService = {}

tagService.storedTags = {}

function tagService.addTag(object, tag)
    tagService.storedTags[object] = tag
end

function tagService.getTag(object)
    return tagService.storedTags[object]
end

function tagService.removeTag(object)
    tagService.storedTags[object] = nil
end

function tagService.hasTag(object, tag)
    return tagService.storedTags[object] == tag
end

function tagService.getAllObjectsWithTag(tag)
    local objects = {}
    for obj, objTag in pairs(tagService.storedTags) do
        if objTag == tag then
            table.insert(objects, obj)
        end
    end
    return objects
end

return tagService
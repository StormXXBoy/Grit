local task = {}

task.scheduled = {}

-- spawn a coroutine task
function task.spawn(fn)
    local co = coroutine.create(fn)
    coroutine.resume(co)
    return co
end

-- yield the current coroutine for a certain amount of seconds
function task.wait(seconds)
    local co = coroutine.running()
    if not co then
        error("task.wait must be called inside task.spawn()")
    end

    table.insert(task.scheduled, {
        time = seconds,
        co = co
    })

    coroutine.yield()
end

-- delay a function call by a certain amount of seconds
function task.delay(seconds, fn)
    table.insert(task.scheduled, {
        time = seconds,
        fn = fn
    })
end

-- put this into your update function
function task.update(dt)
    for i = #task.scheduled, 1, -1 do
        local t = task.scheduled[i]
        t.time = t.time - dt

        if t.time <= 0 then
            table.remove(task.scheduled, i)

            if t.co then -- resume coroutine
                local ok, err = coroutine.resume(t.co)
                if not ok then
                    error(err)
                end
            elseif t.fn then -- call delayed function
                t.fn()
            end
        end
    end
end

return task
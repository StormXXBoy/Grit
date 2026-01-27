local delayService = {}

delayService.delayedTasks = {}

function delayService.addDelay(taskFunction, delayTime)
    local executeTime = os.clock() + delayTime
    table.insert(delayService.delayedTasks, {func = taskFunction, time = executeTime})
end

function delayService.update()
    local currentTime = os.clock()
    local tasksToExecute = {}

    for i = #delayService.delayedTasks, 1, -1 do
        local task = delayService.delayedTasks[i]
        if currentTime >= task.time then
            table.insert(tasksToExecute, task.func)
            table.remove(delayService.delayedTasks, i)
        end
    end

    for _, taskFunc in ipairs(tasksToExecute) do
        taskFunc()
    end
end

return delayService
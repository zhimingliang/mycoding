--状态管理类   --用来管理任务状态机的运转 
StateManage={CurrentState,PreState}         --有当前状态 前一个状态
FightStateRoot={RootStateName,state}             --根状态 
NoFightStateRoot={RootStateName,state}          --根状态
NoActionState={StateName}                   --静止挂机状态
NormalAttackNoActionState={StateName}       --普通攻击状态








 function clone(ClassType)
    temp={}
    for key,value in pairs(ClassType) do
        temp[key]=value        
    end
    return temp
end

function Copy(dest, tab)
  for key, val in pairs(tab) do
    dest[key] = val
  end
end

  
----------------------状态机管理类的new函数---------------------
function stateManage.new()
    local t = clone(stateManage)   
    return t
end
--设置 当前状态 
function stateManage.SetState(self,state)
   self.PreState=CurrentState
   self.CurrentState=state
end

--返回当前状态
function stateManage.GetState(self,state)
   return self.CurrentState.state           --返回最底层 子状态
end



--执行函数
function stateManage.Excute(self,Value)
   --根据 传入的 变量和 当前的状态 执行对应函数
   
   if self.CurrentState.RootStateName == "非战斗状态" and Value.name == "敌人攻击" then 
   --关闭非战斗状态 EndRootState
   self.CurrentState:EndRootState()
   self.CurrentState=Fight                   --进入 到战斗状态 
   --   到战斗状态 的 BeginRootState 
   self.CurrentState:BeginRootState()
   else if self.CurrentState.RootStateName == "战斗状态" and Value.name == "环境安全" then 
   --关闭战斗状态 EndRootState
   self.CurrentState.EndRootState()
   self.CurrentState=NoFight                   --进入 到非战斗状态 
   --   到战斗状态 的 BeginRootState 
   self.CurrentState:BeginRootState()
   
    --不用大状态 切换  执行Excute
   self.CurrentState:RootExcute(Value)                    -- 执行 对应状态的 Excute函数 
end

---------------------战斗根状态--------------------------
function FightStateRoot.new(name)
    local t = clone(StateRoot) 
    t.StateName=name            --根状态的名字
    return t
end

--进入 根state 
function FightStateRoot.BeginRootState(self)
    --创建初始的子状态 普通攻击状态
    self.state=PlayerNomalFight
    self.state:BeginState()                  --调用子状态的状态入口函数
end
--退出 根state
function FightStateRoot.EndRootState(self)
    --退出根状态之前 要先退出子状态
   self.state:EndState()
    --退出根状态  可能返回 子状态名称 等..
end

--Excute
function FightStateRoot.RootExcute(self,value)
   --根据value  和 子状态 判断是不是 要切换子状态 
   --如果切换的 话  调用state:endstate()  state重新赋值   调用state:beginState()
   --然后都要 将参数传入 子类的 Excute()
   self.state:Excute(value)
end

---------------------非战斗根状态--------------------------
function NoFightStateRoot.new(name)
    local t = clone(StateRoot) 
    t.StateName=name            --根状态的名字
    return t
end

--进入 根state 
function NoFightStateRoot.BeginRootState(self)
    --创建初始的子状态 静止挂机状态
    self.state=NoActionState
    self.state:BeginState()                  --调用子状态的状态入口函数
end
--退出 根state
function NoFightStateRoot.EndRootState(self)
    --退出根状态之前 要先退出子状态
    self.state:EndState()
    --退出根状态  可能返回 子状态名称 等..
end
--Excute
function NoFightStateRoot.RootExcute(self,value)
   --根据value  和 子状态 判断是不是 要切换子状态 
   --如果切换的 话  调用state:endstate()  state重新赋值   调用state:beginState()
   --然后都要 将参数传入 子类的 Excute()
   self.state:Excute(value)
end





---------------------子状态(普通攻击状态)---------------

function NormalAttackNoActionState.new()
    local t = clone(NormalAttackNoActionState) 
      FightStateRoot.__index=FightStateRoot
      setmetatable(t, FightStateRoot)               --继承
    t.StateName.StateName="普通攻击状态"
    return t
end
--beginState
function NormalAttackNoActionState.BeginState(self)
    
end
--Excute
function NormalAttackNoActionState.Excute(self,Value)
    
end

--EndState
function NormalAttackNoActionState.EndState(self)
    
end
--------------------子状态(静止挂机状态)------------------ 
function NoActionState.new()
    local t = clone(NoActionState)   
    NoFightStateRoot.__index=NoFightStateRoot
      setmetatable(t, NoFightStateRoot)               --继承
    StateName.StateName="静止挂机状态"
    return t
end
--beginState
function NoActionState.BeginState(self)
   
end
--EndState
function NoActionState.EndState(self)
    
end
--Excute
function NoActionState.Excute(self,Value)
    
end



m_manage= stateManage.new()                     --创建一个 状态机管理类 对象 
Fight=FightStateRoot.new("战斗状态")                         --创建一个 战斗状态 
NoFight=NoFightStateRoot.new("非战斗状态")          --创建一个 非战斗状态 
PlayerNomalFight=NormalAttackNoActionState.new()        --创建一个普通攻击状态

PlayerStatic= NoActionState.new()           --创建一个静止挂机状态


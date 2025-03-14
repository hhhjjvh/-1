using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using System;

/// <summary>
/// 游戏内日期时间结构体（支持年/月/日/时/分/秒）
/// </summary>
[System.Serializable]
public struct GameDateTime
{
    public int year;     // 年份（从1开始）
    public int month;    // 月份（1-12）
    public int day;      // 日期（1-31，根据月份调整）
    public int hour;     // 小时（0-23）
    public int minute;   // 分钟（0-59）
    public float second; // 秒（0-59.999...）

    /// <summary>
    /// 增加指定秒数并标准化时间
    /// </summary>
    public void AddSeconds(float deltaSeconds)
    {
        second += deltaSeconds;
        NormalizeTime();
    }

    /// <summary>
    /// 时间标准化处理（自动进位）
    /// </summary>
    public void NormalizeTime()
    {
        // 秒 -> 分钟进位
        minute += (int)(second / 60);
        second %= 60;

        // 分钟 -> 小时进位
        hour += minute / 60;
        minute %= 60;

        // 小时 -> 天进位
        day += hour / 24;
        hour %= 24;

        // 处理月份和年份进位（需循环处理跨月情况）
        while (day > GetDaysInMonth(month, year))
        {
            day -= GetDaysInMonth(month, year);
            month++;
            if (month <= 12) continue;
            month -= 12;
            year++;
        }
    }

    /// <summary>
    /// 获取指定月份的天数（考虑闰年）
    /// </summary>
    /// <param name="currentMonth">目标月份（1-12）</param>
    /// <param name="currentYear">目标年份（用于闰年计算）</param>
    public int GetDaysInMonth(int currentMonth, int currentYear)
    {
        return currentMonth switch
        {
            2 => IsLeapYear(currentYear) ? 29 : 28, // 二月天数（闰年29天）
            4 or 6 or 9 or 11 => 30,  // 小月
            _ => 31,                   // 大月
        };
    }

    /// <summary>
    /// 闰年判断（符合格里高利历规则）
    /// 规则：能被4整除但不能被100整除，或能被400整除
    /// </summary>
   public static bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
    }

    /// <summary>
    /// 格式化时间字符串（yyyy-MM-dd HH:mm:ss.ff）
    /// </summary>
    public override string ToString()
    {
        return $"{year:0000}-{month:00}-{day:00} {hour:00}:{minute:00}:{second:00.0}";
    }
}

/// <summary>
/// 时间管理系统（单例模式）
/// 功能包括：
/// 1. 游戏时间流速控制
/// 2. 实体时间缩放管理
/// 3. 时间冻结功能
/// 4. 定时事件调度
/// </summary>
public class TimeManager : MonoBehaviour,ISaveManager
{
    // 单例实例
    public static TimeManager Instance { get; private set; }

    [Header("时间配置")]
    [SerializeField] private GameDateTime _initialDateTime; // 初始日期时间
    [SerializeField] private float _baseTimeScale = 1f;     // 基准时间流速
    [SerializeField] private float _timeScaleLerpSpeed = 5f;// 时间缩放过渡速度

    [Header("实体时间缩放系数")]
    [SerializeField] private float _globalTimeScale = 1f;   // 全局基础缩放
    [SerializeField] private float _characterTimeScale = 1f;// 角色时间缩放
    [SerializeField] private float _enemyTimeScale = 1f;    // 敌人时间缩放
    [SerializeField] private float _trapTimeScale = 1f;     // 陷阱时间缩放

    [Header("时间冻结状态")]
    [SerializeField] private bool _freezeGlobalTime;    // 全局时间冻结
    [SerializeField] private bool _freezeCharacters;    // 角色冻结
    [SerializeField] private bool _freezeEnemies;       // 敌人冻结
    [SerializeField] private bool _freezeTraps;         // 陷阱冻结

    [Header("游玩时间统计")]
    [SerializeField] private bool _countPlaytimeWhenPaused = false; // 是否在暂停时统计时间
    private float _totalPlaytimeSeconds;          // 总游玩时间（秒）
    private DateTime _lastSaveTime;               // 最后一次保存时间戳

    // 运行时状态
    private GameDateTime _currentDateTime;       // 当前游戏时间
    private float _targetTimeScale;              // 目标时间流速
    private float _currentTimeScale;             // 当前实际时间流速
    private List<TimeEvent> _scheduledEvents = new List<TimeEvent>(); // 预定事件列表

    // 属性访问器
    public GameDateTime CurrentTime => _currentDateTime; // 当前时间（只读）
    public float WorldTimeScale => _currentTimeScale;     // 当前世界时间流速

    private void Awake()
    {
        // 单例模式初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        transform.SetParent(null); // 清除父对象
        DontDestroyOnLoad(gameObject);
        InitializeTimeSystem();
       
    }

    /// <summary>
    /// 初始化时间系统
    /// </summary>
    private void InitializeTimeSystem()
    {
        _currentDateTime = _initialDateTime;
        _targetTimeScale = _baseTimeScale;
        _currentTimeScale = _baseTimeScale;
        Time.fixedDeltaTime = 0.02f * _currentTimeScale; // 初始化物理时间步长
    }

    private void Update()
    {
        UpdateTimeScaling();         // 更新时间缩放过渡
        if (!_freezeGlobalTime)
            UpdateWorldTime(Time.unscaledDeltaTime); // 使用未缩放时间推进游戏时间
        CheckScheduledEvents();     // 检查预定事件
        UpdatePlaytimeCounting();   // 更新游玩时间统计
    }
    /// <summary>
    /// 更新游玩时间统计
    /// </summary>
    private void UpdatePlaytimeCounting()
    {
        bool shouldCount = _countPlaytimeWhenPaused || !_freezeGlobalTime;
        if (shouldCount) _totalPlaytimeSeconds += Time.unscaledDeltaTime;
    }

    /// <summary>
    /// 获取格式化的总游玩时间
    /// </summary>
    public string GetFormattedPlaytime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_totalPlaytimeSeconds);
        return $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    

    /// <summary>
    /// 平滑过渡时间缩放
    /// </summary>
    private void UpdateTimeScaling()
    {
        _currentTimeScale = Mathf.Lerp(
            _currentTimeScale,
            _targetTimeScale,
            _timeScaleLerpSpeed * Time.unscaledDeltaTime // 使用未缩放时间进行插值
        );
        Time.timeScale = _currentTimeScale;               // 设置Unity时间缩放
        Time.fixedDeltaTime = 0.02f * _currentTimeScale;  // 同步物理时间步长
    }

    /// <summary>
    /// 更新游戏世界时间
    /// </summary>
    /// <param name="deltaTime">未缩放的时间增量</param>
    private void UpdateWorldTime(float deltaTime)
    {
        // 应用全局时间缩放系数
        float effectiveDelta = deltaTime * _globalTimeScale;
        _currentDateTime.AddSeconds(effectiveDelta);
    }

    /// <summary>
    /// 设置全局时间流速
    /// </summary>
    /// <param name="scale">目标流速（1=正常）</param>
    /// <param name="instant">是否立即生效</param>
    public void SetGlobalTimeScale(float scale, bool instant = false)
    {
        _targetTimeScale = scale;
        if (instant) _currentTimeScale = scale;
    }

    /// <summary>
    /// 重置时间为基准流速
    /// </summary>
    public void ResetTimeScale(bool instant = false)
    {
        SetGlobalTimeScale(_baseTimeScale, instant);
    }

    /// <summary>
    /// 获取实体时间缩放系数（考虑全局缩放和冻结状态）
    /// </summary>
    public float GetEntityTimeScale(EntityType entityType)
    {
        if (IsEntityFrozen(entityType)) return 0f;

        // 获取基础缩放系数
        float baseScale = entityType switch
        {
            EntityType.Character => _characterTimeScale,
            EntityType.Enemy => _enemyTimeScale,
            EntityType.Trap => _trapTimeScale,
            _ => 1f
        };

        // 综合全局缩放、实体缩放和当前时间流速
        return baseScale * _globalTimeScale * _currentTimeScale;
    }

    /// <summary>
    /// 检查实体是否被冻结
    /// </summary>
    public bool IsEntityFrozen(EntityType entityType)
    {
        return entityType switch
        {
            EntityType.Character => _freezeCharacters,
            EntityType.Enemy => _freezeEnemies,
            EntityType.Trap => _freezeTraps,
            EntityType.Global => _freezeGlobalTime,
            _ => false
        };
    }

    /// <summary>
    /// 设置实体类型冻结状态
    /// </summary>
    public void FreezeEntityType(EntityType entityType, bool freeze)
    {
        switch (entityType)
        {
            case EntityType.Character: _freezeCharacters = freeze; break;
            case EntityType.Enemy: _freezeEnemies = freeze; break;
            case EntityType.Trap: _freezeTraps = freeze; break;
            case EntityType.Global: _freezeGlobalTime = freeze; break;
        }
    }

    /// <summary>
    /// 添加定时事件（触发时间单位为秒）
    /// </summary>
    public void ScheduleEvent(TimeEvent timeEvent)
    {
        _scheduledEvents.Add(timeEvent);
        // 按触发时间排序（升序）
        _scheduledEvents = _scheduledEvents.OrderBy(e => e.triggerTime).ToList();
    }

    /// <summary>
    /// 检查并执行到期事件（倒序遍历避免索引问题）
    /// </summary>
    private void CheckScheduledEvents()
    {
        for (int i = _scheduledEvents.Count - 1; i >= 0; i--)
        {
            var e = _scheduledEvents[i];
            if (GetTotalElapsedSeconds() >= e.triggerTime)
            {
                e.action.Invoke();
                _scheduledEvents.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 获取游戏启动后的总流逝秒数
    /// </summary>
    public float GetTotalElapsedSeconds()
    {
        int totalDays = 0;
        // 计算历史年份天数
        for (int y = 1; y < _currentDateTime.year; y++)
            totalDays += GameDateTime.IsLeapYear(y) ? 366 : 365;

        // 计算本年已过月份天数
        for (int m = 1; m < _currentDateTime.month; m++)
            totalDays += _currentDateTime.GetDaysInMonth(m, _currentDateTime.year);

        totalDays += _currentDateTime.day - 1; // 当前月已过天数

        return totalDays * 86400 +          // 天数转秒
               _currentDateTime.hour * 3600 +    // 小时转秒
               _currentDateTime.minute * 60 +    // 分钟转秒
               _currentDateTime.second;         // 当前秒数
    }

    /// <summary>
    /// 跳转到指定时间（自动标准化）
    /// </summary>
    public void JumpToDateTime(GameDateTime newDateTime)
    {
        newDateTime.NormalizeTime(); // 先规范化时间数据
        _currentDateTime = newDateTime;
    }

    public void LoadData(GameData data)
    {
        _totalPlaytimeSeconds = data._totalPlaytimeSeconds;
        _lastSaveTime = DateTime.Now;
    }

    public void SaveData(ref GameData data)
    {
        data._totalPlaytimeSeconds = _totalPlaytimeSeconds;
        _lastSaveTime = DateTime.Now;
    }

    /// <summary>
    /// 定时事件结构体
    /// </summary>
    public struct TimeEvent
    {
        public float triggerTime;   // 触发时间（总秒数）
        public UnityAction action;  // 触发动作
    }

    /// <summary>
    /// 实体类型枚举
    /// </summary>
    public enum EntityType { Character, Enemy, Trap, Global }
}
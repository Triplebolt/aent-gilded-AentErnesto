To whoever's holding the keys now,

So. Someone finally landed "Refactor UpdateQuality into per-item-type methods." My one
loop is four now — `UpdateSulfuras`, `UpdateAgedBrie`, `UpdateBackstagePass`,
`UpdateNormalItem`. Fine. I'll say it once: those nested ifs *worked*. Shipped for years.
Don't let anyone tell you otherwise.

But — grudgingly — it's better. You can read what Sulfuras does without tracing five `!=`
checks. And they pinned the behavior *before* moving a line: `BackstagePass_AfterTheConcert_DropsToZero`
proves the passes crater to zero exactly the way I built them. Tests that lock in what the
code does, not what someone wishes it did. That's the part I never bothered with. Right call.

One warning you do not get to ignore: the golden master is the law. That thirty-day output
is the truth now, not the spec. If something looks like a bug, it's probably load-bearing —
customers depend on it either way. Change it deliberately, regenerate, diff. Never hand-edit
it green.

And don't touch the `Item` class. The goblin's still watching.

— Leeroy

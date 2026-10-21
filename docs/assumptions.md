# Assumptions and gap resolutions

The spec leaves these open. Each is a deliberate, reviewable choice.

| # | Gap | Decision | Alternative considered |
|---|---|---|---|
| 1 | Does the vehicle multiplier apply to flat fees? | Yes, to every policy (bus Early Bird = $30). | Hourly only; rejected because the spec says all base rates are car-relative. |
| 2 | Are Early Bird / Night Owl weekday-only? | No. Only the peak surcharge is weekday-gated. | Weekday-only specials; not stated in spec. |
| 3 | Source of public holidays | Injected `IHolidayCalendar`, default empty. | Hard-coded list; rejected as untestable and region-specific. |
| 4 | Which date decides "weekday" for blocks crossing midnight or multi-day stays | Each peak window is checked on its own calendar date. | Date of entry only; wrong for multi-day stays. |
| 5 | Exactly 24h00m | Not "more than 24h", so specials stay valid. | Treat 24h as invalid. |
| 6 | Zero-length stay / exit before entry | Zero bills one hour minimum; exit before entry throws. | Zero fee. |
| 7 | Where loyalty tier comes from | On the ticket; default `None` = 0%. | Separate customer lookup (YAGNI). |
| 8 | Rounding | Exact `decimal`, round to cents (away from zero) at the end of each quote. | Round per step. |
| 9 | Time zone / DST | Local wall-clock `DateTime`; DST out of scope. | `DateTimeOffset`/zones. |
| 10 | Ties | Earlier registered policy wins; result names the policy. | Random/undefined. |
| 11 | Scope | Billing engine plus thin ticket and spot model. No capacity, allocation or persistence. | Full parking-lot simulation (YAGNI). |
| 12 | Night Owl entry "to 23:59:59" | Equivalent to before next midnight (exclusive). | Whole-second granularity only. |
| 13 | 24h rule redundancy | Encoded explicitly in shared flat-rate logic anyway. | Rely on window rules only. |

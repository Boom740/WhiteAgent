# Product Backlog

**Version:** 1.0 | **Last Updated:** 2026-09-22

> รวม User Story ทั้งหมดของโปรเจกต์ — ยังไม่ได้แปลว่าต้องทำใน Sprint นี้ทั้งหมด
> โปรเจกต์นี้แบ่งงานตลอดเทอมเป็น **4 Sprint** (Sprint 1-4) — Sprint ไหนหยิบ Story ไปทำ ให้ใส่เลข Sprint นั้น (1-4) ลงคอลัมน์ `Sprint`

## Must Have (MVP)

| # | User Story                                          | Acceptance Criteria                                                                                                                                                                | Estimate (SP) | Sprint |
| - | --------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a player, I will attack to kill the enemy       | กดปุ่มโจมตีแล้วศัตรูได้รับดาเมจเลือดลดจนตายได้                                                                                       | 12hr          | 1      |
| 2 | As a player, I will throw head to stunt the enemy | ทำการปาหัวออกไปแล้วศัตรูเข้าสู่สถานะมึนงง และหัวเด้งออกจากตัวศัตรูทันที                                      | 6hr           | 1      |
| 3 | As a player, I will take damage                     | เมื่อศัตรูโจมตีแล้วเลือดของผู้เล่นลดลงตามดาเมจที่ได้รับ                                                                     | 3hr           | 1      |
| 4 | As a player, I want to move                         | เมื่อผู้เล่นกด WASD ตัวละครต้องเคลื่อนที่ตามทิศทางที่กดจริงๆ                                                                 | 3hr           | 1      |
| 5 | As a player, I want to save game                    | เมื่อผู้เล่นไปยังเลเวลถัดไป จะทำการบันทึกตำแหน่งไว้ ถ้าผู้เล่นตายด่านไหนจะเกิดใหม่ด่านนั้น | 6hr           | 2      |
| 6 | As a player, I will get gameover                    | เมื่อ HP ของผู้เล่นหรือ HP ของร่างกายหมด                                                                                                           | 1hr           | 2      |
| 7 | As a player, I want to go next level                | เมื่อผู้เล่นเคลียร์ศัตรูในด่านจนหมด                                                                                                             | 3hr           | 2      |
| 8 | As a player, I will fight more enemy                | มีศัตรูเพิ่มขึ้น                                                                                                                                                   | 5 days        | 2 - 3  |

## Should Have

| # | User Story                                   | Acceptance Criteria                                                                                                           | Estimate (SP) | Sprint |
| - | -------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a designer, I want to make Main menu      | มีปุ่มเริ่มเกม มีปุ่มออกเกม มี How to play                                                        | 7hr           | 3      |
| 2 | As a enemy, It will drop item when die       | เมื่อศัตรูตายจะดรอปของ                                                                                  | 6hr           | 3      |
| 3 | As a player, I want to upgrade ability       | เมื่อจบด่านจะให้เลือกอัปเกรด                                                                      | 5 days        | 3      |
| 4 | As a player, I will gather money            | เมื่อศัตรูตายแล้ว มีโอกาสจะดรอปไอเทม                                                       | 1 days        | 3      |
| 5 | As a designer, I want player to hear SFX/BGM | ผู้เล่นได้ยินเสียงในเกม                                                                                | 2 days        | 2 - 3  |
| 6 | As a player, I need some rest after fight    | เมื่อเคลียร์ด่านสำเร็จ ผู้เล่นจะไม่ไปด่านต่อไปจนกว่าจะเดินไปเอง | 1 days        | 3      |
| 7 | As a player, I will fight the boss           | เมื่อถึงท้ายด่านผู้เล่น                                                                                | 3 days        | 3      |

## Nice to Have

| # | User Story                                                                                                      | Acceptance Criteria                                                                                                                                      | Estimate (SP) | Sprint |
| - | --------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- | ------ |
| 1 | As a designer, I want enemy spawn rate stored in a data file, so that I can tune difficulty without recompiling | ปรับค่า spawn rate ในไฟล์ data แล้วรันเกมใหม่ ค่าที่เปลี่ยนมีผลทันทีโดยไม่ต้อง build ใหม่ | 1 week        | -      |
| 2 | As a designer, I want enemy spawn in random position                                                            | ทุกครั้งที่เล่น ศัตรูจะเกิดไม่ซ้ำที่เดิม                                                                          | 1 week        | -      |
| 3 | As a designer, I want game to have setting in game                                                             | ในเกมจะมีระบบ Setting                                                                                                                       | 1 week        | -      |
| 4 | As a designer, I want to make game cut scene                                                                    | ก่อนเข้าหน้าเมนูจะมี cut scene เล่นให้ดูก่อน                                                                            | 1 week        | -      |
| 5 | As a player, I want a inventory to keep item                                                                    | จะมีกระเป๋าหรือกล่องให้ผู้เล่นเก็บไอเทม                                                                           | 1 week        | -      |

## MoSCoW Legend

- **Must Have** — จำเป็นต่อ core gameplay loop เกมเล่นไม่ได้ถ้าขาด (MVP)
- **Should Have** — เพิ่มคุณภาพเกม แต่เกมเล่นได้โดยไม่มีก็ได้
- **Nice to Have** — ทำถ้ามีเวลาเหลือ

## Links

- [[docs/gdd/00-concept|GDD Concept]]
- [[docs/agile/02-sprint-backlog|Sprint Backlog]].

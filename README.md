# UTC Login Automation

Project kiểm thử giao diện đăng nhập Văn phòng điện tử UTC bằng C#, NUnit, Selenium WebDriver, Page Object Model và Allure.

## Giới Thiệu hệ thống

Hệ thống được kiểm thử: <https://vanphongdientu.utc.edu.vn/Login>. Phạm vi hiện có là validation, đăng nhập thất bại bằng dữ liệu giả và trạng thái ô mật khẩu. Không có tài khoản hợp lệ, nên project không khẳng định luồng đăng nhập thành công. Selector và thông báo của sáu ca chức năng dựa trên [DOM và hành vi đã quan sát](docs/DOM_Observation.md).

Hai ca SQL injection dùng dấu nháy đơn như một probe đầu vào và **chỉ được chạy trên staging được phép kiểm thử**. Chúng kiểm tra form còn hiển thị và không lộ chẩn đoán SQL trên giao diện; kết quả đạt không chứng minh hệ thống hoàn toàn không có SQL injection. Hai ca rate limiting và khả năng chịu tải được đặc tả trong Excel, chờ ngưỡng, SLA và môi trường thử được xác nhận. Không tạo tải DDoS lên trang production. Thiết kế tham chiếu [OWASP WSTG về SQL injection](https://wstg.owasp.org/latest/4-Web_Application_Security_Testing/07-Injection/05-SQL_Injection/) và [OWASP WSTG về giới hạn đăng nhập](https://wstg.owasp.org/latest/4-Web_Application_Security_Testing/04-Authentication/03-Weak_Lock_Out_Mechanism/).

Lần chạy chức năng `20261008_162923_814`: **6 Passed, 0 Failed, 0 Skipped**. Lần chạy kiểm tra điều kiện security `20261008_164757_853`: hai ca SQLi **Blocked** trước khi mở URL; hai ca rate limiting/tải **Not Run**. Sáu ca chức năng cũng là Not Run trong lần chạy security vì bị loại bởi bộ lọc. [Excel](test-case/Login_TestCases.xlsx) giữ cả hai Run ID; sheet `Summary` tổng hợp lần chạy gần nhất.

## Cấu Trúc Thư Mục

```text
UTC.LoginTests/
  Configuration/       Đọc URL, timeout, headless; chặn security probe trên production
  Base/                Vòng đời Chrome, screenshot lỗi, thao tác WebDriverWait
  Pages/               LoginPage và locator đã khảo sát
  Helpers/             Giới hạn số lần gửi form, ánh xạ Test Case ID
  TestData/            Username và password giả
  Tests/               Mỗi ca chức năng/SQLi một file, LoginScenario dùng chung
  appsettings.json     Cấu hình mặc định
  allureConfig.json    Cấu hình dữ liệu Allure.NUnit
test-case/
  Login_TestCases.xlsx TestCases, Execution và Summary
docs/
  DOM_Observation.md  Bằng chứng DOM và phản hồi server
  security/            Điều kiện và bước cho ca rate limiting / tải được phê duyệt
scripts/
  run-tests.ps1        Tạo Run ID và chạy nhóm chức năng hoặc security
  update_execution.py Đọc TRX/Allure rồi cập nhật Excel
  create_workbook.py   Tạo workbook mới; ghi đè lịch sử nếu chạy lại
  add_security_cases.py  Bổ sung idempotent ca security vào workbook cũ
runs/                  Artifact theo lần chạy; được loại khỏi Git
```

## Hướng Dẫn Cài Đặt & Thiết Lập (Setup)

Yêu cầu: .NET 10 SDK, Google Chrome và kết nối mạng khi NuGet/Selenium Manager cần tải dependency. Project dùng NUnit 3.14.0, Selenium.WebDriver và Selenium.Support 4.50.0, Allure.NUnit 2.15.0. Python 3 với `openpyxl` chỉ cần khi cập nhật Excel; Java và Node.js chỉ cần khi tạo HTML Allure bằng CLI.

```powershell
git clone https://github.com/LeNguyenAnhDu/web-selenium.git
cd web-selenium
dotnet restore .\UTC.LoginTests\UTC.LoginTests.csproj
dotnet build .\UTC.LoginTests\UTC.LoginTests.csproj
py -m pip install openpyxl
```

Giá trị mặc định nằm trong [`appsettings.json`](UTC.LoginTests/appsettings.json). Có thể ghi đè bằng `UTC_LOGIN_URL`, `UTC_TIMEOUT_SECONDS`, `UTC_HEADLESS` và `UTC_MAX_SUBMISSIONS`. Test chạy tuần tự; mặc định tối đa năm lần gửi form trong một process và dừng khi thấy CAPTCHA, khóa hoặc giới hạn truy cập.

**Security setup:** Khi có staging được phép thử, đặt `UTC_SECURITY_URL` là URL Login staging và `UTC_SECURITY_TESTS_ENABLED=true`. `SecurityTarget` chặn hostname production; URL phải là HTTPS hoặc HTTP loopback. Hiện chưa có staging nên không đặt các biến này. Hai ca rate limiting/tải trong `docs/security/` vẫn cần ngưỡng và SLA được xác nhận trước khi triển khai thực thi.

## Hướng Dẫn Chạy Kiểm Thử & Xem Báo Cáo Allure

Chạy trực tiếp sáu test chức năng bằng .NET, không cần thư mục `scripts`:

```powershell
dotnet test .\UTC.LoginTests\UTC.LoginTests.csproj --filter "TestCategory!=Security"
```

Để tạo Run ID riêng, lưu TRX/Allure và cập nhật Excel, đóng workbook nếu đang mở rồi chạy:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-tests.ps1
powershell -ExecutionPolicy Bypass -File .\scripts\run-tests.ps1 -Headed
```

Lệnh đầu chạy Chrome headless; lệnh thứ hai mở cửa sổ Chrome. Mỗi lần chạy tạo `runs/<Run ID>/results.trx`, `allure-results/` và screenshot nếu có lỗi. `update_execution.py` chỉ ghi Passed khi TRX có kết quả Passed. NUnit `Inconclusive` kèm lý do `Blocked:` được ghi **Blocked** trong Excel; NUnit Skipped khác được giữ **Skipped**.

Khi có staging được phép kiểm thử, chạy **riêng** nhóm SQLi; không dùng URL production:

```powershell
$env:UTC_SECURITY_TESTS_ENABLED = "true"
$env:UTC_SECURITY_URL = "https://STAGING-DUOC-PHEP/Login" # thay bằng URL thật đã được cấp phép
powershell -ExecutionPolicy Bypass -File .\scripts\run-tests.ps1 -Security
```

Nếu thiếu staging hoặc cờ bật, hai test tự dừng trước khi mở URL và trả `Blocked`. `-Security` chỉ chọn các test có category `Security`; hai ca rate limiting/tải vẫn chưa có phương thức NUnit. Không chạy `create_workbook.py` trên workbook đã có lịch sử vì script tạo mới và ghi đè file.

`Allure.NUnit` tạo dữ liệu JSON; **Allure CLI** mới tạo/mở báo cáo HTML. Lần chạy security chỉ có các ca Blocked trước bước mở trang nên không sinh JSON Allure; trạng thái và lý do vẫn có trong TRX/Excel. Cài CLI theo [hướng dẫn Allure cho Node.js](https://allurereport.org/docs/v2/install-for-nodejs/) nếu máy chưa có, rồi dùng Run ID có dữ liệu Allure:

```powershell
npm.cmd install -g allure-commandline
$runId = "20261008_162923_814" # thay bằng Run ID vừa chạy
npm.cmd exec --yes --package=allure-commandline -- allure generate ".\runs\$runId\allure-results" -o ".\runs\$runId\allure-report" --clean
npm.cmd exec --yes --package=allure-commandline -- allure open ".\runs\$runId\allure-report"
```

Trên máy đã dùng để kiểm tra project, Allure CLI 2.46.1 đã tạo HTML thành công cho run chức năng trên. Báo cáo gồm Test Case ID, nhóm, severity, mô tả, bước, thời gian, trạng thái và lỗi. Screenshot lỗi được chụp trước khi đóng Chrome; log không ghi mật khẩu. `runs/` là artifact cục bộ và không được push lên Git.

## Danh Sách các kịch Bản Kiểm Thử

| ID | Kịch bản | Automation | Trạng thái có bằng chứng |
|---|---|---|---|
| TC-LOGIN-001 | Bỏ trống username và password | NUnit | Passed, run `20261008_162923_814` |
| TC-LOGIN-002 | Có username giả, bỏ trống password | NUnit | Passed, cùng run |
| TC-LOGIN-003 | Bỏ trống username, có password giả | NUnit | Passed, cùng run |
| TC-LOGIN-004 | Tài khoản giả nhận lỗi xác thực | NUnit | Passed, cùng run |
| TC-LOGIN-005 | Hai trường chỉ chứa khoảng trắng | NUnit | Passed, cùng run |
| TC-LOGIN-006 | Password dùng `input type=password` | NUnit | Passed, cùng run |
| SEC-LOGIN-001 | Dấu nháy đơn trong username, kiểm tra lỗi SQL lộ ra UI | NUnit, staging only | Blocked, run `20261008_164757_853`; chưa có staging |
| SEC-LOGIN-002 | Dấu nháy đơn trong password, kiểm tra lỗi SQL lộ ra UI | NUnit, staging only | Blocked, cùng run; chưa có staging |
| SEC-LOGIN-003 | Giới hạn thử đăng nhập liên tiếp theo ngưỡng phê duyệt | Chưa triển khai; [đặc tả](docs/security/SEC-LOGIN-003.md) | Not Run; thiếu staging/ngưỡng |
| SEC-LOGIN-004 | Duy trì dịch vụ theo load profile và SLA phê duyệt | Chưa triển khai; [đặc tả](docs/security/SEC-LOGIN-004.md) | Not Run; thiếu môi trường/SLA |

Toàn bộ dữ liệu, bước và cơ sở kỳ vọng nằm trong [Login_TestCases.xlsx](test-case/Login_TestCases.xlsx). `SEC-LOGIN-001/002` chỉ là probe đầu vào đơn giản; kiểm thử SQL injection đầy đủ cần phạm vi và môi trường riêng. Các ca `SEC-LOGIN-003/004` không được đánh Passed khi chưa có kết quả đo thực tế.

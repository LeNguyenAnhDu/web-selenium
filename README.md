# Kiểm thử đăng nhập Văn phòng điện tử UTC

Project Selenium C# + NUnit + Page Object Model cho validation và đăng nhập thất bại tại <https://vanphongdientu.utc.edu.vn/Login>. Không cần tài khoản hợp lệ. [Khảo sát DOM và hành vi](docs/DOM_Observation.md) là cơ sở của assertion.

## Công cụ và cấu trúc

- .NET 10 SDK; Chrome; Python 3 với `openpyxl` để cập nhật Excel; Java và Allure CLI để tạo HTML.
- NUnit 3.14.0, NUnit3TestAdapter 5.0.0, Selenium.WebDriver/Support 4.50.0, Allure.NUnit 2.15.0. Allure.NUnit 2.15.0 khai báo dependency NUnit 3.13.3 trở lên trong [NuGet package](https://www.nuget.org/packages/Allure.NUnit/2.15.0).
- `UTC.LoginTests/Configuration`: URL, timeout, headless, giới hạn gửi form; `Base`: driver và thao tác chờ; `Pages`: locator/luồng login; `Tests` và `TestData`: assertion/dữ liệu; `Helpers`: bộ đếm gửi form; `docs`: khảo sát.

## Chạy test

```powershell
dotnet restore .\UTC.LoginTests\UTC.LoginTests.csproj
py -m pip install openpyxl
powershell -ExecutionPolicy Bypass -File .\scripts\run-tests.ps1
```

Chỉ cần chạy NUnit mà không cập nhật Excel: `dotnet test .\UTC.LoginTests\UTC.LoginTests.csproj`. Trang chạy test ASP.NET Core không nằm trong source hiện tại.

Script tạo `runs/<Run ID>/` riêng cho mỗi lần chạy, đặt `ALLURE_CONFIG` trỏ tới cấu hình Allure của lần đó, chạy NUnit, lưu `results.trx`, `allure-results/` và screenshot khi có test lỗi, rồi bổ sung dòng Execution trong `test-case/Login_TestCases.xlsx`. Không trộn kết quả giữa các lần chạy. Chạy Chrome có giao diện:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\run-tests.ps1 -Headed
```

`UTC_LOGIN_URL`, `UTC_TIMEOUT_SECONDS`, `UTC_HEADLESS`, `UTC_MAX_SUBMISSIONS` có thể ghi đè `appsettings.json`. Mặc định headless, timeout 12 giây và tối đa năm lần gửi form trên một process. Test tuần tự; nếu trang xuất hiện CAPTCHA/khóa/rate limiting, các test phụ thuộc việc gửi form được đánh dấu Inconclusive trong NUnit và **Blocked** trong Excel, không tiếp tục thử vượt cơ chế đó. Test thiếu điều kiện hay không có kết quả trong TRX không được đánh Passed.

## Excel

`test-case/Login_TestCases.xlsx` là workbook XLSX thật có ba sheet: `TestCases`, `Execution`, `Summary`. Có filter, freeze header, wrap text, công thức tổng hợp và trạng thái bằng chữ kèm màu. Sáu dòng `INITIAL` lưu trạng thái `Not Run` ban đầu; mỗi lần chạy thêm sáu dòng có Run ID. Sheet Summary lấy lần chạy gần nhất và đếm bằng công thức Excel. Mở bằng Microsoft Excel để tính lại công thức. Nếu tạo workbook từ đầu, chạy `py .\scripts\create_workbook.py` **trước lần chạy đầu tiên**; lệnh này ghi đè lịch sử Execution hiện có. `update_execution.py` đọc TRX, chỉ đánh Passed khi NUnit thực sự trả Passed, giữ Skipped riêng với Blocked và ánh xạ Allure JSON theo Test Case ID.

## Allure

`Allure.NUnit` tạo JSON trong `allure-results/`, còn **Allure CLI** tạo và mở HTML. Trên máy đã xác minh Allure CLI 2.46.1 qua npm và tạo báo cáo HTML. Nếu máy chưa có CLI, cài theo [hướng dẫn chính thức cho Node.js](https://allurereport.org/docs/v2/install-for-nodejs/) bằng `npm.cmd install -g allure-commandline` (cần Java). Dùng `npm.cmd` trong PowerShell khi execution policy chặn `npm.ps1`.

```powershell
$runId = "20261008_162923_814" # thay bằng Run ID vừa chạy
npm.cmd exec --yes --package=allure-commandline -- allure generate ".\runs\$runId\allure-results" -o ".\runs\$runId\allure-report" --clean
npm.cmd exec --yes --package=allure-commandline -- allure open ".\runs\$runId\allure-report"
```

Allure có tên test, ID, nhóm chức năng, severity, mô tả, các bước, thời gian, trạng thái và lỗi. Screenshot lỗi được đính kèm trước khi đóng Chrome; nếu chụp ảnh lỗi, lỗi chụp chỉ được ghi log. Log không chứa mật khẩu. Khi tất cả test Passed sẽ không có screenshot lỗi.

## Đối chiếu và kết quả đã chạy

Run ID `20261008_162923_814`: Windows / Chrome 154 headless; NUnit 6 Passed, 0 Failed, 0 Skipped. TRX: `runs/20261008_162923_814/results.trx`; Allure JSON: `runs/20261008_162923_814/allure-results/`; HTML: `runs/20261008_162923_814/allure-report/index.html`. Các thư mục `runs/` giữ ở workspace và được loại khỏi Git vì là artifact theo lần chạy.

| Test Case ID | NUnit test | Excel Execution | Allure | Commit |
|---|---|---|---|---|
| TC-LOGIN-001 | `BothFieldsEmptyShowsServerValidation` / `TC-LOGIN-001_BothFieldsEmpty` | Passed | `testCaseId=TC-LOGIN-001` | `d35d489` |
| TC-LOGIN-002 | `PasswordEmptyShowsServerValidation` / `TC-LOGIN-002_PasswordEmpty` | Passed | `testCaseId=TC-LOGIN-002` | `427fef0` |
| TC-LOGIN-003 | `UsernameEmptyShowsServerValidation` / `TC-LOGIN-003_UsernameEmpty` | Passed | `testCaseId=TC-LOGIN-003` | `d6dc7a8` |
| TC-LOGIN-004 | `FakeAccountShowsAuthenticationError` | Passed | `testCaseId=TC-LOGIN-004` | `382ddff` |
| TC-LOGIN-005 | `WhitespaceFieldsShowAuthenticationError` | Passed | `testCaseId=TC-LOGIN-005` | `9596040` |
| TC-LOGIN-006 | `PasswordInputIsMasked` | Passed | `testCaseId=TC-LOGIN-006` | `9c3c784` |

Giới hạn: không kiểm thử đăng nhập thành công, tài khoản hợp lệ, trim đầu/cuối, hoặc vượt độ dài vì thiếu điều kiện/yêu cầu xác nhận. Nhánh screenshot đã được kiểm tra riêng bằng một trang `data:` cục bộ cố ý thiếu form: test báo Failed, lưu PNG có Test Case ID và đính kèm PNG vào Allure; kết quả chẩn đoán này không được nhập vào Execution của lần chạy thật. Nhánh Blocked được kiểm tra riêng với `UTC_MAX_SUBMISSIONS=0`: NUnit trả `NotExecuted` với lý do `Blocked:` và script ánh xạ thành Blocked. Nội dung lỗi server có thể đổi; khi đó cập nhật tài liệu quan sát và assertion theo bằng chứng mới.

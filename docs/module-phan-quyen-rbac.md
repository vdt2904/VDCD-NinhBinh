# Module Phan Quyen RBAC

## 1. Muc tieu
- Quan ly truy cap Admin theo vai tro thay vi hard-code.
- Tach ro quyen theo nhom chuc nang.
- Co san ham seed du lieu demo de test nhanh.

## 2. Vai tro he thong
- `SuperAdmin`: Toan quyen, quan ly user/role va cau hinh he thong.
- `ContentManager`: Quan ly noi dung (bai viet, du an, SEO, contact, auto post...).
- `HrManager`: Quan ly nhan su, co cau to chuc, job/cv.
- `Viewer`: Chi xem dashboard.

## 3. Thanh phan chinh
- Dinh nghia role: `VDCD.Entities/Security/AdminRoles.cs`
- Bang role-user: `user_roles` (map entity `VDCD.Entities/Custom/UserRole.cs`)
- Service role: `VDCD.Business/Service/UserRoleService.cs`
- Service seed demo: `VDCD.Business/Service/RbacDemoSeedService.cs`
- Auth + claim role khi login: `VDCD.Cloud/Areas/Admin/Controllers/AccountController.cs`
- Cau hinh middleware + tao bang role: `VDCD.Cloud/Program.cs`
- UI quan tri user (chon role, seed demo): `VDCD.Cloud/Areas/Admin/Views/User/Index.cshtml`

## 4. Ham insert du lieu demo
Module da co ham:

- `RbacDemoSeedService.SeedDemoUsers(bool resetPasswords = false)`

Tinh chat:
- Idempotent: chay lai khong tao trung user.
- Neu user da ton tai:
  - Van cap nhat role.
  - Chi reset password khi `resetPasswords = true` hoac user chua co password.

## 5. Tai khoan demo duoc tao
Password mac dinh: `Demo@123`

- `demo.superadmin` -> `SuperAdmin`
- `demo.content` -> `ContentManager`
- `demo.hr` -> `HrManager`
- `demo.viewer` -> `Viewer`

## 6. Cach chay seed demo

### Cach 1: Tren giao dien Admin
- Dang nhap bang tai khoan `SuperAdmin`.
- Vao trang `/Admin/User/Index`.
- Bam nut `Seed demo RBAC`.

### Cach 2: Goi API truc tiep
- Endpoint: `POST /admin/account/seed-demo-rbac?resetPassword=false`
- Yeu cau role: `SuperAdmin`.

Vi du (tu browser console khi da login):

```javascript
fetch('/admin/account/seed-demo-rbac?resetPassword=false', { method: 'POST' })
  .then(r => r.json())
  .then(console.log);
```

## 7. Tu dong seed khi startup (tu chon)
Co the bat trong file cau hinh:

```json
"Security": {
  "SeedDemoRbacOnStartup": true,
  "ResetDemoRbacPasswordsOnStartup": false
}
```

Luu y:
- Chi nen bat o moi truong dev/staging.
- Production nen de `false` va goi seed chu dong khi can.

## 8. Checklist test nhanh
- Login bang tung tai khoan demo va kiem tra menu theo role.
- Thu truy cap URL khong du quyen, he thong phai chuyen den trang denied.
- Kiem tra bang `user_roles` da map dung 1 role/user.

## 9. Luu y bao mat
- Doi mat khau demo truoc khi deploy production.
- Tat startup seed tren production.
- Kiem soat tai khoan `SuperAdmin` va han che chia se credential.

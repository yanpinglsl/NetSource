using BlazorDemo.Client.Service;
using BlazorDemo.Client.Shared;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Pages
{
        public partial class CreateUser
        {
            [Inject]
            public IUserHttpRepository UserHttpRepository { get; set; }

            [Inject]
            public NavigationManager NavigationManager { get; set; }
            [Parameter]
            public string DepartmentId { get; set; }

            private SuccessNotification _notification;

            [Parameter]
            public int UserId { get; set; }

            public int DeptId { get; set; }

            public Userinfo Userinfo { get; set; } = new Userinfo();

            public List<DeptInfo> DeptInfos { get; set; } = new List<DeptInfo>();

            public string Message { get; set; }
            public bool Saved { get; set; }
            public string CssClass { get; set; }

            protected override async Task OnInitializedAsync()
            {
                if (UserId != 0)
                {
                    Userinfo = await UserHttpRepository.GetUserinfosById(UserId);
                    DeptId = Userinfo.DeptId;
                }

                DeptInfos = await UserHttpRepository.GetDeptInfos();


            }

            public async Task HandleValidSubmit()
            {
                //var departmentId = int.Parse(DepartmentId);
                if (UserId > 0)
                {
                    await UserHttpRepository.UpdateUser(Userinfo);

                    Saved = true;
                    Message = "修改成功";
                    CssClass = "alert alert-success";
                }
                else
                {
                    var userinfo = await UserHttpRepository.AddUserinfo(Userinfo);
                    if (userinfo != null)
                    {
                        _notification.Show();
                        //Saved = true;
                        //Message = "新增成功";
                        //CssClass = "alert alert-success";
                    }
                    else
                    {
                        Saved = false;
                        Message = "新增失败";
                        CssClass = "alert alert-danger";
                    }
                }
            }

            public void HandleInvalidSubmit()
            {
                CssClass = "alert alert-danger";
                Message = "表单验证失败";
            }

            [Inject]
            public IJSRuntime Js { get; set; }

            private async Task<bool> DeleteEmployee()
            {
                var confirmed = await Js.InvokeAsync<bool>("confirm", $"确定要删除用户?");
                if (confirmed)
                {
                    if (UserId != 0)
                    {
                        await UserHttpRepository.DeleteUser(UserId.ToString());
                    }
                    Saved = true;
                    Message = "删除成功";
                    CssClass = "alert alert-success";
                }
                return false;

            }
            private void GoBack()
            {
                NavigationManager.NavigateTo("/users");
            }

            private void AssignImageUrl(string[] imgUrls)
            {

            }

            private void AssignFileUrl(string path)
            {

            }
        }
    }

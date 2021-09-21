using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using LT.DigitalOffice.GUI.Services.ApiClients.UserService;

namespace LT.DigitalOffice.GUI.Shared.UsersWindow
{
    public partial class Users
    {
        [Parameter]
        public List<UserInfo> UsersInfo { get; set; }

        [Parameter]
        public Dictionary<int, bool> HideEmployees { get; set; }

        [Parameter]
        public Dictionary<int, bool> StatesSelectedEmployees { get; set; }

        public List<UserInfo> AddFoundEmployees()
        {
            List<UserInfo> selectedEmployees = new();

            foreach (int key in StatesSelectedEmployees.Keys)
            {
                if (StatesSelectedEmployees[key])
                {
                    HideEmployees[key] = true;
                    selectedEmployees.Add(UsersInfo[key]);

                    StatesSelectedEmployees[key] = false;
                }
            }

            return selectedEmployees;
        }

        public List<Guid> RemoveSelectedEmployees(out List<UserInfo> employees)
        {
            List<Guid> userIds = new();

            for (int i = StatesSelectedEmployees.Count - 1; i >= 0; i--)
            {
                if (StatesSelectedEmployees[i])
                {
                    userIds.Add(UsersInfo[i].Id);
                    UsersInfo.RemoveAt(i);

                    StatesSelectedEmployees[i] = false;
                }
            }

            employees = UsersInfo;

            return userIds;
        }
    }
}

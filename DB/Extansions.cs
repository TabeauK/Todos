using System.Collections.Generic;

namespace Todos
{
    public static class Extansions
    {
        public static List<UserDTO> ConvertToDTOList(this IEnumerable<User> users)
        {
            List<UserDTO> usersDTO = new();
            foreach (User user in users)
            {
                usersDTO.Add(user.CreateDTO());
            }
            return usersDTO;
        }

        public static List<TaskDTO> ConvertToDTOList(this IEnumerable<Task> tasks)
        {
            List<TaskDTO> tasksDTO = new();
            foreach (Task task in tasks)
            {
                tasksDTO.Add(task.CreateDTO());
            }
            return tasksDTO;
        }

        public static List<CheckDTO> ConvertToDTOList(this IEnumerable<Check> checks)
        {
            List<CheckDTO> checksDTO = new();
            foreach (Check check in checks)
            {
                checksDTO.Add(check.CreateDTO());
            }
            return checksDTO;
        }
    }
}

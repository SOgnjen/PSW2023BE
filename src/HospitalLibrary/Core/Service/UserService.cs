using HospitalLibrary.Core.Model;
using HospitalLibrary.Core.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalLibrary.Core.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        void IUserService.Create(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                throw new ArgumentException(string.Join(Environment.NewLine, errorMessages));
            }

            _userRepository.Create(user);
        }

        public void Delete(User user)
        {
            var existingUser = ((IUserService)this).GetById(user.Id);

            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} does not exist.");
            }

            _userRepository.Delete(existingUser);
        }

        IEnumerable<User> IUserService.GetAll()
        {
            return _userRepository.GetAll();
        }

        User IUserService.GetById(int id)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} does not exist.");
            }

            return user;
        }

        void IUserService.Update(User user)
        {
            var existingUser = ((IUserService)this).GetById(user.Id);

            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} does not exist.");
            }

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                throw new ArgumentException(string.Join(Environment.NewLine, errorMessages));
            }

            _userRepository.Update(user);
        }

    }
}

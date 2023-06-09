using HospitalLibrary.Core.Model;
using HospitalLibrary.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalLibrary.Core.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly HospitalDbContext _context;

        public UserRepository(HospitalDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} does not exist.");
            }

            return user;
        }

        public void Create(User user)
        {
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                var errorMessages = validationResults.Select(r => r.ErrorMessage);
                throw new ArgumentException(string.Join(Environment.NewLine, errorMessages));
            }

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            var existingUser = GetById(user.Id);

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

            _context.Entry(existingUser).State = EntityState.Detached;

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public void Delete(User user)
        {
            var existingUser = _context.Users.Find(user.Id);

            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {user.Id} does not exist.");
            }

            _context.Users.Remove(existingUser);
            _context.SaveChanges();
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace EventCatalog.Models
{
    public class EventFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название")]
        [StringLength(200)]
        [Display(Name = "Название")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Укажите дату")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Range(0, 1000000, ErrorMessage = "Цена не может быть меньше 0")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Range(1, 100000, ErrorMessage = "Мест должно быть больше 0")]
        [Display(Name = "Количество мест")]
        public int Seats { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }
    }
}
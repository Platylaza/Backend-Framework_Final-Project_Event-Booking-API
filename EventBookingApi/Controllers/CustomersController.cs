using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using EventBookingApi.Models;
using EventBookingApi.DTOs;
using EventBookingApi.Services;

namespace EventBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomersController(
            ICustomerService customerService,
            IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all customers.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CustomerReadDto>))]
        public ActionResult<IEnumerable<CustomerReadDto>> GetAll() =>
            Ok(_mapper.Map<IEnumerable<CustomerReadDto>>(_customerService.GetAll()));

        /// <summary>
        /// Returns a customer by its ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerReadDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CustomerReadDto> GetById(int id)
        {
            var customer = _customerService.GetById(id);
            return customer == null ? NotFound(new { message = "Customer not found" }) : _mapper.Map<CustomerReadDto>(customer);
        }

        /// <summary>
        /// Adds a new customer.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CustomerReadDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<CustomerReadDto> Add([FromBody] CustomerCreateDto newCustomerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Customer customer = _mapper.Map<Customer>(newCustomerDto);
            var created = _customerService.Add(customer);

            CustomerReadDto readDto = _mapper.Map<CustomerReadDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = readDto.Id }, readDto);
        }

        /// <summary>
        /// Updates an existing customer.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(int id, [FromBody] CustomerCreateDto updatedCustomerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_customerService.Update(id, _mapper.Map<Customer>(updatedCustomerDto)))
                return NoContent();

            return BadRequest();
        }
    }
}
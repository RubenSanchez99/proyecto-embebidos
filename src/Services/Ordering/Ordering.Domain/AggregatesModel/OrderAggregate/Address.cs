using System;
using EventFlow.ValueObjects;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    public class Address : ValueObject
    {
        public String Street { get; }
        public String City { get; }
        public String State { get; }
        public String Country { get; }
        public String ZipCode { get; }

        private Address() { }

        public Address(string street, string city, string state, string country, string zipcode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }
    }
}

using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3817
{
    public class Itinerary
    {
        public override bool Equals(object obj)
        {
            Itinerary toCompare = obj as Itinerary;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ItineraryId, default(int)) &&
                Object.Equals(toCompare.ItineraryId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ItineraryId, toCompare.ItineraryId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            //return 100;
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ItineraryId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ItineraryId.GetHashCode();
            return hashCode;
        }

        public Itinerary()
            : base()
        {
            this.ItineraryGuests = new HashSet<ItineraryGuest>();
            this.Reservations = new HashSet<Reservation>();
        }

        public virtual int ItineraryId { get; set; }

        public virtual string ItineraryNumber { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ItineraryGuest> ItineraryGuests { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<Reservation> Reservations { get; set; }
    }

    public class ItineraryGuest
    {
        public override bool Equals(object obj)
        {
            ItineraryGuest toCompare = obj as ItineraryGuest;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ItineraryGuestId, default(int)) &&
                Object.Equals(toCompare.ItineraryGuestId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ItineraryGuestId, toCompare.ItineraryGuestId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            //return 90;
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ItineraryGuestId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ItineraryGuestId.GetHashCode();
            return hashCode;
        }

        public ItineraryGuest()
            : base()
        {
            this.ReservationGuests = new HashSet<ReservationGuest>();
        }

        public virtual int ItineraryGuestId { get; set; }

        public virtual string LastName { get; set; }

        public virtual string FirstName { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual Itinerary Itinerary { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationGuest> ReservationGuests { get; set; }
    }

    public class Reservation
    {
        public override bool Equals(object obj)
        {
            Reservation toCompare = obj as Reservation;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ReservationId, default(int)) &&
                Object.Equals(toCompare.ReservationId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ReservationId, toCompare.ReservationId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            //return 80;
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ReservationId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ReservationId.GetHashCode();
            return hashCode;
        }

        public Reservation()
            : base()
        {
            this.ReservationGuests = new HashSet<ReservationGuest>();
            this.ReservationDays = new HashSet<ReservationDay>();
            this.ReservationDayPrices = new HashSet<ReservationDayPrice>();
        }

        public virtual int ReservationId { get; set; }

        public virtual string ReservationNumber { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual Itinerary Itinerary { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationGuest> ReservationGuests { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationDayPrice> ReservationDayPrices { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationDay> ReservationDays { get; set; }
    }

    public class ReservationGuest
    {
        public override bool Equals(object obj)
        {
            ReservationGuest toCompare = obj as ReservationGuest;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ReservationGuestId, default(int)) &&
                Object.Equals(toCompare.ReservationGuestId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ReservationGuestId, toCompare.ReservationGuestId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            //return 70;
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ReservationGuestId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ReservationGuestId.GetHashCode();
            return hashCode;
        }

        public ReservationGuest()
            : base()
        {
            this.ReservationDayShares = new HashSet<ReservationDayShare>();
        }

        public virtual int ReservationGuestId { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual Reservation Reservation { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual ItineraryGuest ItineraryGuest { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationDayShare> ReservationDayShares { get; set; }
    }

    public class ReservationDay
    {
        public override bool Equals(object obj)
        {
            ReservationDay toCompare = obj as ReservationDay;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ReservationDayId, default(int)) &&
                Object.Equals(toCompare.ReservationDayId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ReservationDayId, toCompare.ReservationDayId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ReservationDayId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ReservationDayId.GetHashCode();
            return hashCode;
        }

        public ReservationDay()
            : base()
        {
            this.ReservationDayPrices = new HashSet<ReservationDayPrice>();
            this.ReservationDayShares = new HashSet<ReservationDayShare>();
        }

        public virtual int ReservationDayId { get; set; }

        public virtual System.DateTime BusinessDate { get; set; }

        public virtual decimal QuotedRate { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual Reservation Reservation { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationDayPrice> ReservationDayPrices { get; set; }

        /// <summary>
        /// Cascade: AllDeleteOrphan
        /// </summary>
        public virtual ICollection<ReservationDayShare> ReservationDayShares { get; set; }
    }

    public class ReservationDayPrice
    {
        public override bool Equals(object obj)
        {
            ReservationDayPrice toCompare = obj as ReservationDayPrice;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ReservationDayPriceId, default(int)) &&
                Object.Equals(toCompare.ReservationDayPriceId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ReservationDayPriceId, toCompare.ReservationDayPriceId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ReservationDayPriceId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ReservationDayPriceId.GetHashCode();
            return hashCode;
        }

        public virtual int ReservationDayPriceId { get; set; }

        public virtual decimal Price { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual Reservation Reservation { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual ReservationDay ReservationDay { get; set; }
    }

    public class ReservationDayShare
    {
        public override bool Equals(object obj)
        {
            ReservationDayShare toCompare = obj as ReservationDayShare;
            if (toCompare == null)
            {
                return false;
            }

            if (Object.Equals(this.ReservationDayShareId, default(int)) &&
                Object.Equals(toCompare.ReservationDayShareId, default(int)))
                return ReferenceEquals(this, toCompare);

            if (!Object.Equals(this.ReservationDayShareId, toCompare.ReservationDayShareId))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            //return 60;
            int hashCode = 13;

            // if object is transient, use base GetHashCode()
            if (Object.Equals(this.ReservationDayShareId, default(int)))
                return base.GetHashCode();

            hashCode = (hashCode * 7) + ReservationDayShareId.GetHashCode();
            return hashCode;
        }

        public virtual int ReservationDayShareId { get; set; }

        public virtual System.Nullable<decimal> ShareValue { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual ReservationGuest ReservationGuest { get; set; }

        /// <summary>
        /// Cascade: None
        /// </summary>
        public virtual ReservationDay ReservationDay { get; set; }
    }
}
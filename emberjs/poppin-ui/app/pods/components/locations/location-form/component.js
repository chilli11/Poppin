import Component from '@glimmer/component';
import { tracked } from '@glimmer/tracking';
import { action } from '@ember/object';
import { inject as service } from '@ember/service';
import YelpBusinessModel from 'poppin-ui/models/yelp-business';

const hoursDto = {
	sunday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	},
	monday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	},
	tuesday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	},
	wednesday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	},
	thursday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	},
	friday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	},
	saturday: {
		openingHour: null,
		openingMinute: null,
		closingHour: null,
		closingMinute: null
	}
};

export default class LocationFormComponent extends Component {
	@service locationsService;
	@service store;

	@tracked yelpMatches;
	@tracked name;
	@tracked addressLine1;
	@tracked addressLine2;
	@tracked city;
	@tracked state;
	@tracked zip;
	get zipCode() {
		return this.zip ? this.zip.substr(0, 5) : null;
	};
	get zipCodeTrailing() {
		const spl = this.zip ? this.zip.split('-') : [];
		return spl[1] || null;
	};
	@tracked capacity = '0';
	@tracked crowdSize = '0';
	@tracked hoursDto = hoursDto;
	@tracked hours = {
		sunday: {
			opening: null,
			closing: null
		},
		monday: {
			opening: null,
			closing: null
		},
		tuesday: {
			opening: null,
			closing: null
		},
		wednesday: {
			opening: null,
			closing: null
		},
		thursday: {
			opening: null,
			closing: null
		},
		friday: {
			opening: null,
			closing: null
		},
		saturday: {
			opening: null,
			closing: null
		}
	};

	get locationDTO() {
		const { name, capacity, crowdSize, hours } = this;
		return  {
			name,
			address: {
				line1: this.addressLine1,
				line2: this.addressLine2,
				city: this.city,
				state: this.state,
				zipCode: parseInt(this.zipCode, 10),
				zipCodeTrailing: this.zipCodeTrailing ? parseInt(this.zipCodeTrailing) : null
			},
			categories: [],
			capacity: parseInt(this.capacity, 10),
			crowdSize: parseInt(this.capacity, 10),
			hours
		};
	}

	constructor() {
		super(...arguments);
		this.populate();
	}

	populate() {
		if (this.args.location) {
			const { loc } = this.args;
			this.name = loc.Name || null;
			this.addressLine1 = loc.address.line1;
			this.addressLine2 = loc.address.line2;
			this.city = loc.address.city;
			this.state = loc.address.state;
			this.zipCode = loc.address.zipCode;
			this.zipCodeTrailing = loc.address.zipCodeTrailing;
			this.capacity = loc.capacity || 0;
			this.hours = loc.Hours || {
				Sunday: {
					opening: null,
					closing: null
				},
				Monday: {
					opening: null,
					closing: null
				},
				Tuesday: {
					opening: null,
					closing: null
				},
				Wednesday: {
					opening: null,
					closing: null
				},
				Thursday: {
					opening: null,
					closing: null
				},
				Friday: {
					opening: null,
					closing: null
				},
				Saturday: {
					opening: null,
					closing: null
				},
				Sunday: {
					opening: null,
					closing: null
				}
			};
		}
	}

	@action
	submit(e) {
		return this.locationsService.createNewLocation(this.locationDTO)
			.then(({ businesses }) => {
				this.yelpMatches = businesses;
				return this.yelpMatches;
			});
	}
}

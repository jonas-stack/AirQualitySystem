# AI/promptfoo/context.py
def get_var(var_name, prompt, other_vars):
    if var_name == 'context':
        # Return actual air quality information based on the inquiry
        inquiry = other_vars.get('inquiry', '')

        if 'air quality' in inquiry.lower():
            return {
                'output': """
                - CO2 levels above 1000 PPM indicate poor ventilation
                - Humidity between 30-60% is considered healthy
                - PM2.5 levels should remain below 5 µg/m³ for optimal indoor air
                - Ventilation is recommended when CO2 exceeds 1000 PPM
                - Low humidity (below 30%) can cause dry skin and respiratory issues
                """
            }

    # Add more contexts for different inquiries
    return {'output': 'No specific information available for this inquiry.'}